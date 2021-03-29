using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Logging;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public class ExpressionValidationService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly ILog _log;

        public ExpressionValidationService(UnitOfDataPersistenceWork unitOfDataPersistenceWork, ILog log)
        {
            _unitOfWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _log = log ?? throw new ArgumentNullException(nameof(_log));
        }

        public ValidationResult ValidateEquation(EquationValidationParameters model)
        {
            if (model.IsPiecewise)
            {
                return CheckPiecewise(model.Expression);
            }

            try
            {
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
            }
            catch (CalculateEvaluateException e)
            {
                return new ValidationResult { IsValid = false, ValidationMessage = e.Message };
            }
            catch (Exception e)
            {
                return new ValidationResult { IsValid = false, ValidationMessage = e.Message };
            }

            return new ValidationResult { IsValid = true, ValidationMessage = "Success" };
        }

        private ValidationResult CheckPiecewise(string piecewise)
        {
            var ageValues = new Dictionary<int, double>();
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
                return new ValidationResult { IsValid = true, ValidationMessage = "Success" };
            }

            _log.Error("At least one TIME,CONDITION pair must be entered");
            return new ValidationResult
            {
                IsValid = false,
                ValidationMessage = "At least one TIME,CONDITION pair must be entered"
            };
        }

        public CriterionValidationResult ValidateCriterion(string mergedCriteriaExpression, UserCriteriaDTO currentUserCriteriaFilter)
        {
            var expression = mergedCriteriaExpression.Replace("|", "'").ToUpper();
            /*expression = CheckAttributes(expression);
            _calculateEvaluateService.BuildClass(expression, false);
            _calculateEvaluateService.CompileAssembly();*/
            try
            {
                CheckAttributes(expression);
                var attributes = _unitOfWork.Context.Attribute.ToList();
                var compiler = new CalculateEvaluateCompiler();
                foreach (var attribute in attributes.Where(_ => expression.Contains(_.Name)))
                {
                    compiler.ParameterTypes[attribute.Name] = attribute.DataType == "NUMBER"
                        ? CalculateEvaluateParameterType.Number
                        : CalculateEvaluateParameterType.Text;
                }

                compiler.GetEvaluator(expression);
            }
            catch (CalculateEvaluateException e)
            {
                return new CriterionValidationResult { IsValid = false, ResultsCount = 0, ValidationMessage = e.Message };
            }
            catch (Exception e)
            {
                return new CriterionValidationResult { IsValid = false, ResultsCount = 0, ValidationMessage = e.Message };
            }

            return GetResultsCount(expression, currentUserCriteriaFilter);
        }

        private void CheckAttributes(string target)
        {
            var attributes = _unitOfWork.Context.Attribute.ToList();
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

        public CriterionValidationResult GetResultsCount(string expression, UserCriteriaDTO currentUserCriteriaFilter)
        {
            if (string.IsNullOrEmpty(expression))
            {
                _log.Error("There is no criteria created");
                return new CriterionValidationResult { IsValid = false, ResultsCount = 0, ValidationMessage = "There is no criterion expression." };
            }
            //oracle chokes on non-space whitespace
            var whiteSpaceMechanic = new Regex(@"\s+");

            // Appending the criteria filtering clause for non-admin users
            if (currentUserCriteriaFilter.HasCriteria)
            {
                currentUserCriteriaFilter.Criteria = "(" + currentUserCriteriaFilter.Criteria + ")";

                expression += $" AND { currentUserCriteriaFilter.Criteria }";
            }

            // modify the expression replacing all special characters and white space
            var modifiedExpression = whiteSpaceMechanic.Replace(expression.Replace("[", "").Replace("]", "").Replace("@", ""), " ");
            // parameterize the predicate and add it to the select
            var parameterizedData = ParameterizeExpression(modifiedExpression);

            var strSelect = $"SELECT COUNT(*) FROM SECTION_13 INNER JOIN SEGMENT_13_NS0 ON SECTION_13.SECTIONID = SEGMENT_13_NS0.SECTIONID WHERE {parameterizedData.ParameterString}";
            // create a sql connection
            using var connection = _unitOfWork.GetLegacyConnection();
            try
            {
                // open the connection
                connection.Open();
                // create a sql command with the select string and the connection
                using var cmd = new SqlCommand(strSelect, connection);
                // add the command parameters
                cmd.Parameters.AddRange(parameterizedData.SqlParameters.ToArray());
                // execute the query
                using var dataReader = cmd.ExecuteReader();
                // get the returned count
                var count = dataReader.HasRows && dataReader.Read() ? dataReader.GetValue(0) : 0;

                // return the results
                return new CriterionValidationResult
                {
                    IsValid = (int)count > 0,
                    ResultsCount = (int)count,
                    ValidationMessage = (int)count > 0 ? "Success" : "Invalid"
                };
            }
            catch (SqlException e)
            {
                var message = $"Failed SQL Query: {strSelect}, Error Message: {e.Message}";
                _log.Error(message);
                return new CriterionValidationResult { IsValid = false, ResultsCount = 0, ValidationMessage = message };
            }
            catch (Exception e2)
            {
                _log.Error(e2.Message);
                return new CriterionValidationResult
                {
                    IsValid = false,
                    ResultsCount = 0,
                    ValidationMessage = e2.Message
                };
            }
        }

        public ParameterizedData ParameterizeExpression(string expression)
        {
            var predicateParameters = new List<string>();
            var sqlParameters = new List<SqlParameter>();
            var operators = new List<string>() { "<=", ">=", "<>", "=", "<", ">" };
            var operatorsRegex = new Regex(@"<=|>=|<>|=|<|>");
            var parameterCount = 0;
            var startingIndex = 0;
            var predicates = new List<string>();
            var spacedString = 0;
            var indexForSpacedString = 0;
            var attributes = _unitOfWork.Context.Attribute.Select(_ => _.Name).ToList();

            while (startingIndex < expression.Length)
            {
                var index = expression.IndexOf(" ", startingIndex, StringComparison.Ordinal);

                if (index == -1)
                {
                    if (spacedString == 0)
                    {
                        var length = expression.Length - startingIndex;
                        predicates.Add(expression.Substring(startingIndex, length));
                    }
                    else
                    {
                        var lengthForCustomString = expression.Length - indexForSpacedString;
                        predicates.Add(expression.Substring(indexForSpacedString, lengthForCustomString));
                        /*
                                                spacedString = 0;
                        */
                    }
                    break;
                }

                if (expression[index + 1] == '(' || expression[index + 1] == '['
                    || expression.Substring(index + 1, 3) == "AND"
                    || expression.Substring(index + 1, 2) == "OR")
                {
                    if (spacedString == 0)
                    {
                        var length = index - startingIndex;
                        predicates.Add(expression.Substring(startingIndex, length));
                    }
                    else
                    {
                        var lengthForCustomString = index - indexForSpacedString;
                        predicates.Add(expression.Substring(indexForSpacedString, lengthForCustomString));
                        spacedString = 0;
                    }
                    startingIndex = index + 1;
                }
                else
                {
                    if (spacedString == 0)
                    {
                        indexForSpacedString = startingIndex;
                        spacedString++;
                    }
                    startingIndex = index + 1;
                }
            }
            //var predicates = criteria.Split(' ');
            foreach (var predicate in predicates)
            {
                if (operatorsRegex.IsMatch(predicate))
                {
                    foreach (var @operator in operators)
                    {
                        if (!predicate.Contains(@operator))
                        {
                            continue;
                        }

                        // count number of closed parentheses
                        var closedParenthesesCount = predicate.Count(x => x == ')');
                        // split the predicate at the operator
                        var splitPredicate = predicate.Split(new[] { @operator }, StringSplitOptions.None);
                        // create the parameter name
                        var parameterName = $"@value{++parameterCount}";
                        // create the parameter value
                        var value = splitPredicate[1].Replace(")", "").Replace("'", "");

                        var parameterizedPredicate = !attributes.Contains(value)
                            ? $"{splitPredicate[0]} {@operator} {parameterName}"
                            : $"{splitPredicate[0]} {@operator} {value}";
                        // add a number of closed parentheses equal to closedParenthesesCount to end
                        // of parameterizedPredicate
                        if (closedParenthesesCount > 0)
                        {
                            for (var i = 0; i < closedParenthesesCount; i++)
                            {
                                parameterizedPredicate += ")";
                            }
                        }
                        // add the parameterizedPredicate to the criteriaParameterizedPredicates list
                        predicateParameters.Add(parameterizedPredicate);
                        // create a new sql parameter with parameterName and value
                        sqlParameters.Add(new SqlParameter()
                        {
                            ParameterName = parameterName,
                            Value = value
                        });
                        break;
                    }
                }
                else
                {
                    predicateParameters.Add(predicate);
                }
            }

            return new ParameterizedData
            {
                ParameterString = string.Join(" ", predicateParameters),
                SqlParameters = sqlParameters
            };
        }
    }

    public class ParameterizedData
    {
        public string ParameterString { get; set; }

        public List<SqlParameter> SqlParameters { get; set; }
    }
}
