using System;
using System.Collections.Generic;
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
        public ExcelTreatmentLoader(IExpressionValidationService expressionValidationService )
        {
            _expressionValidationService = expressionValidationService;
        }


        public object Validate(TreatmentDTO treatment)
        {
            var criteria = new UserCriteriaDTO();
            foreach (var cost in treatment.Costs)
            {


            }
            return null;
        }
        private static Dictionary<string, string> DetailsSectionAsDictionary(ExcelWorksheet worksheet)
        {
            var r = new Dictionary<string, string>();
            var costsRowIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Costs, 2);
            for (var i = 2; i < costsRowIndex; i++)
            {
                var name = worksheet.Cells[i, 1].Text.ToLower();
                var value = worksheet.Cells[i, 2].Text;
                r[name] = value;
            }
            return r;
        }

        private static int FindRowWithFirstColumnContent(ExcelWorksheet worksheet, string content, int startIndex)
        {
            var r = startIndex;
            var endIndex = worksheet.Dimension.End.Row;
            var lowerCaseContent = content.ToLowerInvariant();
            while (r <= endIndex && worksheet.Cells[r, 1].Text.ToLowerInvariant() != lowerCaseContent)
            {
                r++;
            }
            if (r == endIndex + 1)
            {
                throw new Exception($"Cell with content {content} not found!");
            }

            return r;
        }

        private static int ParseInt(string s, int defaultValue = 0)
        {
            if (int.TryParse(s, out var r))
            {
                return r;
            }
            return defaultValue;
        }

        private static string ValidationLocation(string worksheetName, int row, int column)
        {
            var r = $"{worksheetName} row {row} column {column}";
            return r;
        }

        private TreatmentCostLoadResult LoadCosts(ExcelWorksheet worksheet)
        {
            var costs = new List<TreatmentCostDTO>();
            var validationMessages = new List<string>();
            var validationCriteria = new UserCriteriaDTO();
            var costsLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Costs, 2);
            var consequencesLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Consequences, costsLineIndex);
            for (var i = costsLineIndex + 2; i < consequencesLineIndex; i++)
            {
                var equation = worksheet.Cells[i, 1].Text;
                var criterion = worksheet.Cells[i, 2].Text;
                if (!string.IsNullOrWhiteSpace(equation))
                {
                    var equationValidationResult = ValidateEquation(validationCriteria, equation);
                    if (!equationValidationResult.IsValid)
                    {
                        validationMessages.Add($"{ValidationLocation(worksheet.Name, i, 1)}: { equationValidationResult.ValidationMessage}");
                    }
                    var equationDto = new EquationDTO
                    {
                        Id = Guid.NewGuid(),
                        Expression = equation,
                    };
                    CriterionLibraryDTO criterionLibrary = null;
                    if (criterion != null)
                    {
                        var validateCriterion = _expressionValidationService.ValidateCriterion(criterion, validationCriteria);
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
            var r = new TreatmentCostLoadResult
            {
                Costs = costs,
                ValidationMessages = validationMessages,
            };
            return r;
        }

        private ValidationResult ValidateEquation(UserCriteriaDTO validationCriteria, string equation)
        {
            var equationValidationParameters = new EquationValidationParameters
            {
                Expression = equation,
                CurrentUserCriteriaFilter = validationCriteria,
                IsPiecewise = false,
            };
            var equationValidationResult = _expressionValidationService.ValidateEquation(equationValidationParameters);
            return equationValidationResult;
        }

        private TreatmentConsequenceLoadResult LoadConsequences(ExcelWorksheet worksheet)
        {
            var consequences = new List<TreatmentConsequenceDTO>();
            var validationCriteria = new UserCriteriaDTO();
            var validationMessages = new List<string>();
            var consequencesRow = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Consequences, 3);
            var height = worksheet.Dimension.End.Row;
            for (int i = consequencesRow + 2; i <= height; i++)
            {
                var attribute = worksheet.Cells[i, 1].Text;
                var equation = worksheet.Cells[i, 3].Text;
                var criterionString = worksheet.Cells[i, 4].Text;
                EquationDTO equationDto = null;
                if (!string.IsNullOrWhiteSpace(equation))
                {
                    var validateEquation = ValidateEquation(validationCriteria, equation);
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
                    var validateCriterion = _expressionValidationService.ValidateCriterion(criterionString, validationCriteria);
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
            var r = new TreatmentConsequenceLoadResult
            {
                Consequences = consequences,
                ValidationMessages = validationMessages,
            };
            return r;
        }

        public TreatmentLoadResult LoadTreatment(ExcelWorksheet worksheet)
        {
            var worksheetName = worksheet.Name;
            var dictionary = DetailsSectionAsDictionary(worksheet);
            var description = dictionary.GetValueOrDefault(TreatmentExportStringConstants.TreatmentDescription.ToLowerInvariant());
            var yearsBeforeSame = dictionary.GetValueOrDefault(TreatmentExportStringConstants.YearsBeforeSame.ToLowerInvariant());
            var yearsBeforeAny = dictionary.GetValueOrDefault(TreatmentExportStringConstants.YearsBeforeAny.ToLowerInvariant());
            var categoryString = dictionary.GetValueOrDefault(TreatmentExportStringConstants.Category.ToLowerInvariant());
            var treatmentCategory = EnumDeserializer.Deserialize<TreatmentDTOEnum.TreatmentCategory>(categoryString);
            var assetTypeString = dictionary.GetValueOrDefault(TreatmentExportStringConstants.AssetType.ToLowerInvariant());
            var assetType = EnumDeserializer.Deserialize<TreatmentDTOEnum.AssetType>(assetTypeString);
            var loadCosts = LoadCosts(worksheet);
            var loadConsequences = LoadConsequences(worksheet);
            var newTreatment = new TreatmentDTO
            {
                Name = worksheetName,
                Id = Guid.NewGuid(),
                Description = description,
                Category = treatmentCategory,
                AssetType = assetType,
                ShadowForAnyTreatment = ParseInt(yearsBeforeAny),
                ShadowForSameTreatment = ParseInt(yearsBeforeSame),
                Costs = loadCosts.Costs,
                Consequences = loadConsequences.Consequences,
            };
            var validationMessages = new List<string>();
            validationMessages.AddRange(loadCosts.ValidationMessages);
            validationMessages.AddRange(loadConsequences.ValidationMessages);
            var r = new TreatmentLoadResult
            {
                Treatment = newTreatment,
                ValidationMessages = validationMessages,
            };
            return r;
        }
    }
}
