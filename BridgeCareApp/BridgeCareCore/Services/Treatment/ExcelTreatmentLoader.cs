using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Utils;
using OfficeOpenXml;

namespace BridgeCareCore.Services.Treatment
{
    public class ExcelTreatmentLoader
    {
        private readonly IExpressionValidationService _expressionValidationService;

        public ExcelTreatmentLoader(IExpressionValidationService expressionValidationService)
        {
            _expressionValidationService = expressionValidationService;
        }

        private static Dictionary<string, string> DetailsSectionAsDictionary(ExcelWorksheet worksheet)
        {
            var returnValue = new Dictionary<string, string>();
            var costsRowIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Costs, 2);
            for (var i = 2; i < costsRowIndex; i++)
            {
                var name = worksheet.Cells[i, 1].Text.ToLower();
                var value = worksheet.Cells[i, 2].Text;
                returnValue[name] = value;
            }
            return returnValue;
        }

        private static int FindRowWithFirstColumnContent(ExcelWorksheet worksheet, string content, int startIndex)
        {
            var returnValue = startIndex;
            var endIndex = worksheet.Dimension.End.Row;
            var lowerCaseContent = content.ToLowerInvariant();
            while (returnValue <= endIndex && worksheet.Cells[returnValue, 1].Text.ToLowerInvariant() != lowerCaseContent)
            {
                returnValue++;
            }
            if (returnValue == endIndex + 1)
            {
                returnValue = (content == TreatmentExportStringConstants.PerformanceFactors) ? 0 : throw new Exception($"Cell with content {content} not found!");
            }

            return returnValue;
        }

        private static int ParseInt(string s, int defaultValue = 0)
        {
            if (int.TryParse(s, out var returnValue))
            {
                return returnValue;
            }
            return defaultValue;
        }

        private static float ParseFloat(string s, int defaultValue = 0)
        {
            if (float.TryParse(s, out var returnValue))
            {
                return returnValue;
            }
            return defaultValue;
        }

        private static string ValidationLocation(string worksheetName, int row, int column)
        {
            var columnName = ExcelCellAddress.GetColumnLetter(column);
            var returnValue = $"Worksheet {worksheetName} cell {columnName}{row}";
            return returnValue;
        }

        private TreatmentCostLoadResult LoadCosts(ExcelWorksheet worksheet)
        {
            var newCriteria = NewCriteria();
            var costs = new List<TreatmentCostDTO>();
            var validationMessages = new List<string>();
            var costsLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Costs, 2);
            var consequencesLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Consequences, costsLineIndex);
            for (var i = costsLineIndex + 2; i < consequencesLineIndex; i++)
            {
                var equation = worksheet.Cells[i, 1].Text;
                var criterion = worksheet.Cells[i, 2].Text;
                if (!string.IsNullOrWhiteSpace(equation))
                {
                    var equationValidationResult = ValidateEquation(equation);
                    if (!equationValidationResult.IsValid)
                    {
                        validationMessages.Add($"{ValidationLocation(worksheet.Name, i, 1)}: {equationValidationResult.ValidationMessage}");
                    }
                    var equationDto = new EquationDTO
                    {
                        Id = Guid.NewGuid(),
                        Expression = equation,
                    };
                    CriterionLibraryDTO criterionLibrary = null;
                    if (!string.IsNullOrWhiteSpace(criterion))
                    {
                        var validateCriterion = _expressionValidationService.ValidateCriterionWithoutResults(criterion, newCriteria);
                        if (!validateCriterion.IsValid)
                        {
                            validationMessages.Add($"{ValidationLocation(worksheet.Name, i, 2)}: {validateCriterion.ValidationMessage}");
                        }
                        criterionLibrary = new CriterionLibraryDTO
                        {
                            Id = Guid.NewGuid(),
                            Name = "from Excel import",
                            MergedCriteriaExpression = criterion,
                            IsSingleUse = true,
                        };
                    }
                    var cost = new TreatmentCostDTO
                    {
                        Id = Guid.NewGuid(),
                        Equation = equationDto,
                        CriterionLibrary = criterionLibrary,
                    };
                    costs.Add(cost);
                }
            }
            var returnValue = new TreatmentCostLoadResult
            {
                Costs = costs,
                ValidationMessages = validationMessages,
            };
            return returnValue;
        }

        private UserCriteriaDTO NewCriteria()
        {
            var returnValue = new UserCriteriaDTO() { UserId = Guid.Empty, CriteriaId = Guid.Empty, Criteria = null, HasCriteria = false };
            return returnValue;
        }

        private ValidationResult ValidateEquation(string equation)
        {
            var newCriteria = NewCriteria();
            var equationValidationParameters = new EquationValidationParameters
            {
                Expression = equation,
                CurrentUserCriteriaFilter = newCriteria,
                IsPiecewise = false,
            };
            var equationValidationResult = _expressionValidationService.ValidateEquation(equationValidationParameters);
            return equationValidationResult;
        }

        private TreatmentConsequenceLoadResult LoadConsequences(ExcelWorksheet worksheet)
        {
            var newCriteria = NewCriteria();
            var consequences = new List<TreatmentConsequenceDTO>();
            var validationMessages = new List<string>();
            var consequencesRow = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Consequences, 3);
            var budgetsLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Budgets, consequencesRow);
            var height = budgetsLineIndex == 0 ? worksheet.Dimension.End.Row : budgetsLineIndex - 1;
            for (var i = consequencesRow + 2; i <= height; i++)
            {
                var attribute = worksheet.Cells[i, 1].Text;
                var equation = worksheet.Cells[i, 3].Text;
                var criterionString = worksheet.Cells[i, 4].Text;
                EquationDTO equationDto = null;
                if (!string.IsNullOrWhiteSpace(equation))
                {
                    var validateEquation = ValidateEquation(equation);
                    if (!validateEquation.IsValid)
                    {
                        validationMessages.Add($"{ValidationLocation(worksheet.Name, i, 3)}: {validateEquation}");
                    }
                    equationDto = new EquationDTO
                    {
                        Id = Guid.NewGuid(),
                        Expression = equation,
                    };
                }
                CriterionLibraryDTO criterionLibraryDto = null;
                if (!string.IsNullOrWhiteSpace(criterionString))
                {
                    var validateCriterion = _expressionValidationService.ValidateCriterionWithoutResults(criterionString, newCriteria);
                    if (!validateCriterion.IsValid)
                    {
                        validationMessages.Add($"{ValidationLocation(worksheet.Name, i, 4)}: {validateCriterion.ValidationMessage}");
                    }
                    criterionLibraryDto = new CriterionLibraryDTO
                    {
                        Id = Guid.NewGuid(),
                        IsSingleUse = true,
                        MergedCriteriaExpression = criterionString,
                        Name = "from Excel import",
                        Description = "",
                    };
                }
                if (!string.IsNullOrWhiteSpace(attribute))
                {
                    var changeValue = worksheet.Cells[i, 2].Text;
                    var consequence = new TreatmentConsequenceDTO
                    {
                        Id = Guid.NewGuid(),
                        Attribute = attribute,
                        ChangeValue = changeValue,
                        Equation = equationDto,
                        CriterionLibrary = criterionLibraryDto,
                    };
                    consequences.Add(consequence);
                }
            }
            var returnValue = new TreatmentConsequenceLoadResult
            {
                Consequences = consequences,
                ValidationMessages = validationMessages,
            };
            return returnValue;
        }

        private TreatmentBudgetsLoadResult LoadBudgets(ExcelWorksheet worksheet, List<BudgetDTO> scenarioBudgets)
        {
            var budgetIds = new List<Guid>();
            var validationMessages = new List<string>();

            var budgetsLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Budgets, 2);
            var PfLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.PerformanceFactors, budgetsLineIndex);
            if (budgetsLineIndex == 0)
            {
                throw new Exception($"Cell with content {TreatmentExportStringConstants.Budgets} not found!");
            }

            for (var i = budgetsLineIndex + 2; i < PfLineIndex; i++)
            {
                var budgetName = worksheet.Cells[i, 1].Text;
                if (!string.IsNullOrEmpty(budgetName))
                {
                    var scenarioBudget = scenarioBudgets.Where(_ => _.Name == budgetName).FirstOrDefault();
                    if (scenarioBudget != null)
                    {
                        budgetIds.Add(scenarioBudget.Id);
                    }
                    else
                    {
                        validationMessages.Add($"{ValidationLocation(worksheet.Name, i, 1)}: {"Invalid budget"}");
                    }
                }
            }

            return new TreatmentBudgetsLoadResult { budgetIds = budgetIds, ValidationMessages = validationMessages };
        }

        private TreatmentPerformanceFactorLoadResult LoadPerformanceFactor(ExcelWorksheet worksheet)
        {
            var performanceFactors = new List<TreatmentPerformanceFactorDTO>();
            var validationMessages = new List<string>();

            var pfLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.PerformanceFactors, 2);
            if (pfLineIndex == 0)
            {
                throw new Exception($"Cell with content {TreatmentExportStringConstants.PerformanceFactors} not found!");
            }

            var height = worksheet.Dimension.End.Row;
            for (var i = pfLineIndex + 2; i <= height; i++)
            {
                var attribute = worksheet.Cells[i, 1].Text;
                var pf = worksheet.Cells[i, 2].Text;

                if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(pf))
                {
                    performanceFactors.Add(new TreatmentPerformanceFactorDTO() { Attribute = attribute, PerformanceFactor = ParseFloat(pf), Id = Guid.NewGuid() });
                }
            }

            return new TreatmentPerformanceFactorLoadResult { PerformanceFactors = performanceFactors, ValidationMessages = validationMessages };
        }

        public TreatmentLoadResult LoadTreatment(ExcelWorksheet worksheet)
        {
            var worksheetName = worksheet.Name;
            var dictionary = DetailsSectionAsDictionary(worksheet);
            var description = dictionary.GetValueOrDefault(TreatmentExportStringConstants.TreatmentDescription.ToLowerInvariant());
            var yearsBeforeSame = dictionary.GetValueOrDefault(TreatmentExportStringConstants.YearsBeforeSame.ToLowerInvariant());
            var yearsBeforeAny = dictionary.GetValueOrDefault(TreatmentExportStringConstants.YearsBeforeAny.ToLowerInvariant());
            var categoryString = dictionary.GetValueOrDefault(TreatmentExportStringConstants.Category.ToLowerInvariant());
            var treatmentCategory = EnumDeserializer.Deserialize<TreatmentCategory>(categoryString);
            var assetTypeString = dictionary.GetValueOrDefault(TreatmentExportStringConstants.AssetType.ToLowerInvariant());
            var assetType = EnumDeserializer.Deserialize<AssetCategories>(assetTypeString);
            var criterion = dictionary.GetValueOrDefault(TreatmentExportStringConstants.Criterion.ToLowerInvariant());
            var performanceFactors = LoadPerformanceFactor(worksheet);
            var loadCosts = LoadCosts(worksheet);
            var loadConsequences = LoadConsequences(worksheet);
            var newTreatment = new TreatmentDTO
            {
                Name = worksheetName,
                Id = Guid.NewGuid(),
                Description = description,
                Category = (TreatmentCategory)treatmentCategory,
                AssetType = (AssetCategories)assetType,
                ShadowForAnyTreatment = ParseInt(yearsBeforeAny),
                ShadowForSameTreatment = ParseInt(yearsBeforeSame),
                Costs = loadCosts.Costs,
                Consequences = loadConsequences.Consequences,
                CriterionLibrary = criterion != default ? new CriterionLibraryDTO()
                {
                    Id = Guid.NewGuid(),
                    MergedCriteriaExpression = criterion,
                    IsSingleUse = true,
                    Name = "Is from import"
                } : new CriterionLibraryDTO(),
                PerformanceFactors = performanceFactors.PerformanceFactors
            };
            var validationMessages = new List<string>();
            validationMessages.AddRange(loadCosts.ValidationMessages);
            validationMessages.AddRange(loadConsequences.ValidationMessages);
            var returnValue = new TreatmentLoadResult
            {
                Treatment = newTreatment,
                ValidationMessages = validationMessages,
            };
            return returnValue;
        }

        public TreatmentLoadResult LoadScenarioTreatment(ExcelWorksheet worksheet, List<BudgetDTO> scenarioBudgets)
        {
            var treatmentLoadResult = LoadTreatment(worksheet);
            var loadBudgets = LoadBudgets(worksheet, scenarioBudgets);

            treatmentLoadResult.Treatment.BudgetIds = loadBudgets.budgetIds;
            treatmentLoadResult.ValidationMessages.AddRange(loadBudgets.ValidationMessages);

            return treatmentLoadResult;
        }


        public TreatmentSupersedeRulesLoadResult LoadTreatmentSupersedeRules(ExcelWorksheet worksheet, List<TreatmentDTO> scenarioTreatments)
        {

            var validationMessages = new List<string>();

            // var budgetsLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Budgets, 2);
            //var PfLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.PerformanceFactors, budgetsLineIndex);
            //if (budgetsLineIndex == 0)
            //{
                throw new Exception($"Cell with content {TreatmentExportStringConstants.TreatmentName} not found!");
           // }



            //return new TreatmentSupersedeRulesLoadResult {  ValidationMessages = validationMessages };
        }
    }
}
