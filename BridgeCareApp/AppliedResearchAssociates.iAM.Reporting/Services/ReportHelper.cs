using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Common;
using System.Data;
using MoreLinq;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Logging;

namespace AppliedResearchAssociates.iAM.Reporting.Services
{
    public class ReportHelper
    {
        private readonly IUnitOfWork _unitOfWork;
        private ILog _log;

        public ReportHelper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public T CheckAndGetValue<T>(IDictionary itemsArray, string itemName)
        {
            var itemValue = default(T);

            if (itemsArray == null) { return itemValue; }
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName)) { return itemValue; }

            if (itemsArray.Contains(itemName)) { itemValue = (T)itemsArray[itemName]; }

            //return value
            return itemValue;
        }

        private static readonly Dictionary<string, string> FunctionalClassDescriptions =
            new Dictionary<string, string>()
            {
                { "01", "01 - Rural - Principal Arterial - Interstate" },
                { "02", "02 - Rural - Principal Arterial - Other" },
                { "03", "03 - Rural - Other Freeway/Expressway" },
                { "06", "06 - Rural - Minor Arterial" },
                { "07", "07 - Rural - Major Collector" },
                { "08", "08 - Rural - Minor Collector" },
                { "09", "09 - Rural - Local" },
                { "NN", "NN - Other" },
                { "11", "11 - Urban - Principal Arterial - Interstate" },
                { "12", "12 - Urban - Principal Arterial - Other Freeway & Expressways" },
                { "14", "14 - Urban - Other Principal Arterial" },
                { "16", "16 - Urban - Minor Arterial" },
                { "17", "17 - Urban - Collector" },
                { "19", "19 - Urban - Local" },
                { "99", "99 - Urban - Ramp" }
            };

        public string FullFunctionalClassDescription(string functionalClassAbbreviation)
        {
            return FunctionalClassDescriptions.ContainsKey(functionalClassAbbreviation) ? FunctionalClassDescriptions[functionalClassAbbreviation] : FunctionalClassDescriptions["NN"];
        }

        public bool BridgeFundingBOF(AssetSummaryDetail section)
        {
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var NBISlen = section.ValuePerTextAttribute["NBISLEN"];

            return
                NBISlen is "Y" &&
                functionalClass is "08" or "09" or "18" or "19";
        }

        public bool BridgeFundingNHPP(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }

            return
                (fedAid is "1" && functionalClass is "01" or "02" or "03" or "06" or "07" or "11" or "12" or "14" or "16" or "17") ||
                (fedAid is "0" && functionalClass is "99");
        }

        public bool BridgeFundingSTP(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);

            return fedAid is "1" or "2";
        }

        public bool BridgeFundingBRIP(AssetSummaryDetail section)
        {
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var NBISlen = section.ValuePerTextAttribute["NBISLEN"];

            return
                NBISlen is "Y" &&
                functionalClass is "01" or "02";
        }

        public bool BridgeFundingState(AssetSummaryDetail section)
        {
            var internetReport = section.ValuePerTextAttribute["INTERNET_REPORT"];

            return internetReport is "State" or "Local";
        }

        public bool BridgeFundingNotApplicable(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }

            return
                (fedAid is "0" && functionalClass is "01" or "02" or "03" or "06" or "07" or "11" or "12" or "14" or "16" or "17") ||
                functionalClass is "NN";
        }

        public HashSet<string> GetPerformanceCurvesAttributes(Simulation simulation)
        {
            var currentAttributes = new HashSet<string>();
            // Distinct performance curve attributes
            foreach (var performanceCurve in simulation.PerformanceCurves)
            {
                currentAttributes.Add(performanceCurve.Attribute.Name);
            }
            return currentAttributes;
        }

        public string GetBenefitAttribute(Simulation simulation) => simulation.AnalysisMethod.Benefit.Attribute.Name;

        public HashSet<string> GetBudgets(List<SimulationYearDetail> years)
        {
            var budgets = new HashSet<string>();
            foreach (var item in years.FirstOrDefault()?.Budgets)
            {
                budgets.Add(item.BudgetName);
            }
            return budgets;
        }

        public List<AssetDetail> GetSectionsWithUnfundedTreatments(SimulationYearDetail simulationYearDetail)
        {
            var untreatedSections =
                    simulationYearDetail.Assets
                        .Where(section => section.TreatmentCause == TreatmentCause.NoSelection && section.TreatmentOptions.Count > 0)
                        .ToList();
            return untreatedSections;
        }

        public List<AssetDetail> GetSectionsWithFundedTreatments(SimulationYearDetail simulationYearDetail)
        {
            var treatedSections = simulationYearDetail.Assets.Where(section => section.TreatmentCause is not TreatmentCause.NoSelection);
            return treatedSections.ToList();
        }

        /// <summary>
        /// Validate criteria expression and filter the reportOutputData based on given criteria,
        /// keep only criteria specific assets in InitialAssetSummaries and Years' Assets
        /// </summary>
        /// <param name="reportOutputData"></param>
        /// <param name="networkId"></param>
        /// <param name="criteria"></param>
        /// <returns>CriteriaValidationResult</returns>
        public CriteriaValidationResult FilterReportOutputData(SimulationOutput reportOutputData, Guid networkId, string criteria)
        {
            try
            {
                _log = new LogNLog();
                if (string.IsNullOrEmpty(criteria))
                {
                    return new CriteriaValidationResult { IsValid = false, ValidationMessage = "There is no criterion expression." };
                }

                CheckAttributes(criteria);

                var modifiedExpression = criteria
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("@", "")
                    .Replace("|", "'")
                    .ToUpper();
                var pattern = "\\[[^\\]]*\\]";
                var rg = new Regex(pattern);
                var match = rg.Matches(criteria);
                var attributeNames = new List<string>();
                foreach (Match m in match)
                {
                    attributeNames.Add(m.Value.Substring(1, m.Value.Length - 2));
                }
                                
                var compiler = new CalculateEvaluateCompiler();
                compiler.GetEvaluator(modifiedExpression);

                var attributes = _unitOfWork.AttributeRepo.GetAttributesWithNames(attributeNames);
                attributes.ForEach(attribute =>
                {
                    compiler.ParameterTypes[attribute.Name] = attribute.Type == "NUMBER"
                        ? CalculateEvaluateParameterType.Number
                        : CalculateEvaluateParameterType.Text;
                });                

                var customAttributes = new List<(string name, string datatype)>();
                foreach (var attribute in attributes)
                {
                    customAttributes.Add((attribute.Name, attribute.Type));
                }

                //TODO get assets and update reportOutputData by filtering
                var assetIds = GetAssetIds(modifiedExpression, customAttributes, networkId);

                return new CriteriaValidationResult
                {
                    IsValid = true,
                    ValidationMessage = "Success"
                };
            }
            catch (CalculateEvaluateException e)
            {
                _log.Error($"{e.Message}\r\n{e.StackTrace}");
                return new CriteriaValidationResult { IsValid = false, ValidationMessage = e.Message };
            }
            catch (Exception e)
            {
                _log.Error($"{e.Message}\r\n{e.StackTrace}");
                return new CriteriaValidationResult { IsValid = false, ValidationMessage = e.Message };
            }
        }

        private void CheckAttributes(string target)
        {
            var attributes = _unitOfWork.AttributeRepo.GetAllAttributesAbbreviated();
            target = target.Replace('[', '?');
            foreach (var allowedAttribute in attributes.Where(allowedAttribute => target.IndexOf("?" + allowedAttribute.Name + "]", StringComparison.Ordinal) >= 0))
            {
                target = allowedAttribute.Type == "STRING"
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

        private List<Guid> GetAssetIds(string expression, List<(string name, string dataType)> attributes, Guid networkId)
        {
            var assetIds = new List<Guid>();
            //var flattenedDataTable = CreateFlattenedDataTable(attributes);

            var attributeNames = new List<string>();
            foreach (var attribute in attributes)
            {
                attributeNames.Add(attribute.name);
            }

            var results =
                _unitOfWork.AggregatedResultRepo.GetAggregatedResultsForAttributeNames(networkId,
                attributeNames);
            var valuePerAttributeNamePerMaintainableAssetId = results
                .GroupBy(_ => _.MaintainableAssetId, _ => _)
                .ToDictionary(_ => _.Key, aggregatedResults =>
                {
                    var value = aggregatedResults.ToDictionary(_ => _.Attribute.Name, _ =>
                    {
                        var data = _.Attribute.Type == AttributeTypeNames.Number
                            ? _.NumericValue?.ToString()
                            : _.TextValue;

                        var type = _.Attribute.Type;
                        return (data, type);
                    });
                    return value;
                });
            assetIds = valuePerAttributeNamePerMaintainableAssetId.Keys.ToList();
            assetIds = results.Select(r=>r.MaintainableAssetId).Distinct().ToList();
//            AddToFlattenedDataTable(flattenedDataTable, valuePerAttributeNamePerMaintainableAssetId);
         //   var dataRows = flattenedDataTable.Select(expression);
            // Retrieve asset ids


            return assetIds;
        }

        private DataTable CreateFlattenedDataTable(List<(string name, string dataType)> attributeNames)
        {
            var flattenedDataTable = new DataTable("FlattenedDataTable");
            flattenedDataTable.Columns.Add("MaintainableAssetId", typeof(Guid));
            attributeNames.ForEach(attributeName =>
            {
                if (!flattenedDataTable.Columns.Contains(attributeName.name))
                {
                    if (attributeName.dataType.Equals("NUMBER", StringComparison.OrdinalIgnoreCase))
                    {
                        flattenedDataTable.Columns.Add(attributeName.name, typeof(double));
                    }
                    else
                    {
                        flattenedDataTable.Columns.Add(attributeName.name, typeof(string));
                    }
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
    }    
}
