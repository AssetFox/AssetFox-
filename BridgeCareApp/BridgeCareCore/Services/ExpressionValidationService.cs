using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Logging;
using BridgeCareCore.Models.Validation;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace BridgeCareCore.Services
{
    public class ExpressionValidationService : IExpressionValidationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILog _log;

        public ExpressionValidationService(IUnitOfWork unitOfDataPersistenceWork, ILog log)
        {
            _unitOfWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _log = log ?? throw new ArgumentNullException(nameof(_log));
        }

        public ValidationResult ValidateEquation(EquationValidationParameters model)
        {
            try
            {
                if (model.IsPiecewise)
                {
                    return CheckPiecewise(model.Expression);
                }

                var expression = model.Expression.Trim();
                CheckAttributes(expression);
                var attributes = _unitOfWork.Context.Attribute.ToList();
                var compiler = new CalculateEvaluateCompiler();
                foreach (var attribute in attributes.Where(_ => expression.Contains(_.Name)))
                {
                    compiler.ParameterTypes[attribute.Name] = attribute.DataType == "NUMBER"
                        ? CalculateEvaluateParameterType.Number
                        : CalculateEvaluateParameterType.Text;
                }

                compiler.GetCalculator(expression);

                return new ValidationResult { IsValid = true, ValidationMessage = "Success" };
            }
            catch (CalculateEvaluateException e)
            {
                return new ValidationResult { IsValid = false, ValidationMessage = e.Message };
            }
            catch (Exception e)
            {
                return new ValidationResult { IsValid = false, ValidationMessage = e.Message };
            }
        }

        private ValidationResult CheckPiecewise(string piecewise)
        {
            var ageValues = new SortedDictionary<int, double>();
            piecewise = piecewise.Trim();

            var pieces = piecewise.Split(new[] { '(' });

            foreach (var piece in pieces)
            {
                if (piece.Length == 0)
                {
                    continue;
                }

                var commaDelimitedPair = piece.TrimEnd(')');
                var ageValuePair = commaDelimitedPair.Split(',');
                int age;
                double value;

                try
                {
                    age = Convert.ToInt32(ageValuePair[0]);
                    value = Convert.ToDouble(ageValuePair[1]);
                }
                catch
                {
                    _log.Error($"Failure to convert TIME,CONDITION pair to (int,double): {commaDelimitedPair}");
                    return new ValidationResult
                    {
                        IsValid = false,
                        ValidationMessage =
                            $"Failure to convert TIME,CONDITION pair to (int,double): {commaDelimitedPair}"
                    };
                }

                if (age < 0)
                {
                    _log.Error("Values for TIME must be 0 or greater");
                    return new ValidationResult
                    {
                        IsValid = false,
                        ValidationMessage = "Values for TIME must be 0 or greater"
                    };
                }

                if (!ageValues.ContainsKey(age))
                {
                    ageValues.Add(age, value);
                }
                else
                {
                    _log.Error("Only unique integer values for TIME are allowed");
                    return new ValidationResult
                    {
                        IsValid = false,
                        ValidationMessage = "Only unique integer values for TIME are allowed"
                    };
                }
            }

            if (ageValues.Count >= 1)
            {
                var previous = ageValues.First();
                foreach (var ageValue in ageValues)
                {
                    if (ageValue.Value > previous.Value)
                    {
                        _log.Error($"Values for CONDITION must descend. Check pairs ({previous.Key},{previous.Value}) and ({ageValue.Key},{ageValue.Value})");
                        return new ValidationResult
                        {
                            IsValid = false,
                            ValidationMessage = $"Values for CONDITION must descend. Check pairs ({previous.Key},{previous.Value}) and ({ageValue.Key},{ageValue.Value})"
                        };
                    }
                    previous = ageValue;
                }

                return new ValidationResult { IsValid = true, ValidationMessage = "Success" };
            }

            _log.Error("At least one TIME,CONDITION pair must be entered");
            return new ValidationResult
            {
                IsValid = false,
                ValidationMessage = "At least one TIME,CONDITION pair must be entered"
            };
        }

        public CriterionValidationResult ValidateCriterion(string mergedCriteriaExpression, UserCriteriaDTO currentUserCriteriaFilter, Guid networkId)
        {
            try
            {
                if (string.IsNullOrEmpty(mergedCriteriaExpression))
                {
                    return new CriterionValidationResult { IsValid = false, ResultsCount = 0, ValidationMessage = "There is no criterion expression." };
                }

                // Appending the criteria filtering clause for non-admin users
                if (currentUserCriteriaFilter.HasCriteria)
                {
                    currentUserCriteriaFilter.Criteria = "(" + currentUserCriteriaFilter.Criteria + ")";

                    mergedCriteriaExpression = $"({mergedCriteriaExpression}) AND { currentUserCriteriaFilter.Criteria }";
                }

                CheckAttributes(mergedCriteriaExpression);

                var modifiedExpression = mergedCriteriaExpression
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("@", "")
                    .Replace("|", "'")
                    .ToUpper();

                var pattern = "\\[[^\\]]*\\]";
                var rg = new Regex(pattern);
                var match = rg.Matches(mergedCriteriaExpression);
                var hashMatch = new List<string>();
                foreach (Match m in match)
                {
                    hashMatch.Add(m.Value.Substring(1, m.Value.Length - 2));
                }

                var attributes = _unitOfWork.AttributeRepo.GetAttributesWithNames(hashMatch);

                var compiler = new CalculateEvaluateCompiler();

                attributes.ForEach(attribute =>
                {
                    compiler.ParameterTypes[attribute.Name] = attribute.Type == "NUMBER"
                        ? CalculateEvaluateParameterType.Number
                        : CalculateEvaluateParameterType.Text;
                });

                compiler.GetEvaluator(modifiedExpression);

                var customAttribute = new List<(string name, string datatype)>();
                foreach (var attribute in attributes)
                {
                    customAttribute.Add((attribute.Name, attribute.Type));
                }

                var resultsCount = GetResultsCount(modifiedExpression, customAttribute, networkId);

                return new CriterionValidationResult
                {
                    IsValid = true, ResultsCount = resultsCount, ValidationMessage = "Success"
                };
            }
            catch (CalculateEvaluateException e)
            {
                _log.Error($"{e.Message}\r\n{e.StackTrace}");
                return new CriterionValidationResult { IsValid = false, ResultsCount = 0, ValidationMessage = e.Message };
            }
            catch (Exception e)
            {
                _log.Error($"{e.Message}\r\n{e.StackTrace}");
                return new CriterionValidationResult { IsValid = false, ResultsCount = 0, ValidationMessage = e.Message };
            }
        }

        public CriterionValidationResult ValidateCriterionWithoutResults(string mergedCriteriaExpression, UserCriteriaDTO currentUserCriteriaFilter)
        {
            try
            {
                if (string.IsNullOrEmpty(mergedCriteriaExpression))
                {
                    return new CriterionValidationResult { IsValid = false, ResultsCount = 0, ValidationMessage = "There is no criterion expression." };
                }

                // Appending the criteria filtering clause for non-admin users
                if (currentUserCriteriaFilter.HasCriteria)
                {
                    currentUserCriteriaFilter.Criteria = "(" + currentUserCriteriaFilter.Criteria + ")";

                    mergedCriteriaExpression = $"({mergedCriteriaExpression}) AND { currentUserCriteriaFilter.Criteria }";
                }

                CheckAttributes(mergedCriteriaExpression);

                var modifiedExpression = mergedCriteriaExpression
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("@", "")
                    .Replace("|", "'")
                    .ToUpper();

                var pattern = "\\[[^\\]]*\\]";
                var rg = new Regex(pattern);
                var match = rg.Matches(mergedCriteriaExpression);
                var hashMatch = new HashSet<string>();
                foreach (Match m in match)
                {
                    hashMatch.Add(m.Value.Substring(1, m.Value.Length - 2));
                }

                var attributes = _unitOfWork.Context.Attribute
                    .Where(_ => hashMatch.Contains(_.Name))
                    .Select(attribute => new AttributeDTO
                    {
                        Name = attribute.Name,
                        Type = attribute.DataType
                    }).AsNoTracking().ToList();

                var compiler = new CalculateEvaluateCompiler();

                attributes.ForEach(attribute =>
                {
                    compiler.ParameterTypes[attribute.Name] = attribute.Type == "NUMBER"
                        ? CalculateEvaluateParameterType.Number
                        : CalculateEvaluateParameterType.Text;
                });

                compiler.GetEvaluator(modifiedExpression);

                return new CriterionValidationResult
                {
                    IsValid = true, ResultsCount = 0, ValidationMessage = "Success"
                };
            }
            catch (CalculateEvaluateException e)
            {
                _log.Error($"{e.Message}\r\n{e.StackTrace}");
                return new CriterionValidationResult { IsValid = false, ResultsCount = 0, ValidationMessage = e.Message };
            }
            catch (Exception e)
            {
                _log.Error($"{e.Message}\r\n{e.StackTrace}");
                return new CriterionValidationResult { IsValid = false, ResultsCount = 0, ValidationMessage = e.Message };
            }
        }

        private void CheckAttributes(string target)
        {
            var attributes = _unitOfWork.Context.Attribute.AsNoTracking().ToList();
            target = target.Replace('[', '?');
            foreach (var allowedAttribute in attributes.Where(allowedAttribute => target.IndexOf("?" + allowedAttribute.Name + "]", StringComparison.Ordinal) >= 0))
            {
                target = allowedAttribute.DataType == "STRING"
                    ? target.Replace("?" + allowedAttribute.Name + "]", "[@" + allowedAttribute.Name + "]")
                    : target.Replace("?" + allowedAttribute.Name + "]", "[" + allowedAttribute.Name + "]");
            }

            if (target.Count(f => f == '?') <= 0)
            {
                return;
            }

            var start = target.IndexOf('?');
            var end = target.IndexOf(']');

            _log.Error("Unsupported Attribute " + target.Substring(start + 1, end - 1));
            throw new InvalidOperationException("Unsupported Attribute " + target.Substring(start + 1, end - 1));
        }

        private DataTable CreateFlattenedDataTable(List<(string name, string dataType)> attributeNames)
        {
            var flattenedDataTable = new DataTable("FlattenedDataTable");
            flattenedDataTable.Columns.Add("MaintainableAssetId", typeof(Guid));
            attributeNames.ForEach(attributeName =>
            {
                if(attributeName.dataType.Equals("NUMBER", StringComparison.OrdinalIgnoreCase))
                {
                    flattenedDataTable.Columns.Add(attributeName.name, typeof(double));
                }
                else
                {
                    flattenedDataTable.Columns.Add(attributeName.name, typeof(string));
                }
            });
            return flattenedDataTable;
        }

        private void AddToFlattenedDataTable(DataTable flattenedDataTable,
            Dictionary<Guid, Dictionary<string, (string data, string type)>> valuePerAttributeNamePerMaintainableAssetId) =>
            valuePerAttributeNamePerMaintainableAssetId.Keys.ForEach(maintainableAssetId =>
            {
                var row = flattenedDataTable.NewRow();
                row["MaintainableAssetId"] = maintainableAssetId;
                valuePerAttributeNamePerMaintainableAssetId[maintainableAssetId].Keys.ForEach(attributeName =>
                {
                    var currData = valuePerAttributeNamePerMaintainableAssetId[maintainableAssetId][attributeName];

                    if (currData.type.Equals("NUMBER", StringComparison.OrdinalIgnoreCase))
                    {
                        if (double.TryParse(valuePerAttributeNamePerMaintainableAssetId[maintainableAssetId][attributeName].data, out var res))
                        {
                            row[attributeName] = res;
                        }
                    }
                    else
                    {
                        row[attributeName] = valuePerAttributeNamePerMaintainableAssetId[maintainableAssetId][attributeName].data;
                    }
                });
                flattenedDataTable.Rows.Add(row);
            });


        private int GetResultsCount(string expression, List<(string name, string dataType)> attributes, Guid networkId)
        {
            var flattenedDataTable = CreateFlattenedDataTable(attributes);

            var attributeNames = new List<string>();
            foreach (var attribute in attributes)
            {
                attributeNames.Add(attribute.name);
            }

            var valuePerAttributeNamePerMaintainableAssetId = _unitOfWork.Context.AggregatedResult.Include(_ => _.MaintainableAsset)
                .Where(_ => attributeNames.Contains(_.Attribute.Name) && _.MaintainableAsset.NetworkId == networkId)
                .Select(aggregatedResult => new AggregatedResultEntity
                {
                    MaintainableAssetId = aggregatedResult.MaintainableAssetId,
                    TextValue = aggregatedResult.TextValue,
                    NumericValue = aggregatedResult.NumericValue,
                    Attribute = new AttributeEntity
                    {
                        Name = aggregatedResult.Attribute.Name,
                        DataType = aggregatedResult.Attribute.DataType
                    }
                }).AsNoTracking().AsSplitQuery().AsEnumerable()
                .GroupBy(_ => _.MaintainableAssetId, _ => _)
                .ToDictionary(_ => _.Key, aggregatedResults =>
                {
                    var value = aggregatedResults.ToDictionary(_ => _.Attribute.Name, _ =>
                    {
                        var data = _.Attribute.DataType == DataPersistenceConstants.AttributeNumericDataType
                            ? _.NumericValue?.ToString()
                            : _.TextValue;

                        var type = _.Attribute.DataType;
                        return (data, type);
                    });
                    return value;
                });

            AddToFlattenedDataTable(flattenedDataTable, valuePerAttributeNamePerMaintainableAssetId);

            return flattenedDataTable.Select(expression).Length;
        }

    }
}
