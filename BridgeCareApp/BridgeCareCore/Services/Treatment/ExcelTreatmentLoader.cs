using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Utils;
using OfficeOpenXml;

namespace BridgeCareCore.Services.Treatment
{
    public static class ExcelTreatmentLoader
    {
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

        private static List<TreatmentCostDTO> LoadCosts(ExcelWorksheet worksheet)
        {
            var r = new List<TreatmentCostDTO>();
            var costsLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Costs, 2);
            var consequencesLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Consequences, costsLineIndex);
            for (var i = costsLineIndex + 2; i < consequencesLineIndex; i++)
            {
                var equation = worksheet.Cells[i, 1].Text;
                var criterion = worksheet.Cells[i, 2].Text;
                if (!string.IsNullOrWhiteSpace(equation))
                {
                    var equationDto = new EquationDTO
                    {
                        Id = Guid.NewGuid(),
                        Expression = equation,
                    };
                    CriterionLibraryDTO criterionLibrary = null;
                    if (criterion != null)
                    {
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
                    r.Add(cost);
                }
            }
            return r;
        }

        private static List<TreatmentConsequenceDTO> LoadConsequences(ExcelWorksheet worksheet)
        {
            var r = new List<TreatmentConsequenceDTO>();
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
                    equationDto = new EquationDTO
                    {
                        Id = Guid.NewGuid(),
                        Expression = equation,
                    };
                }
                CriterionLibraryDTO criterionLibraryDto = null;
                if (!string.IsNullOrWhiteSpace(criterionString))
                {
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
                    r.Add(consequence);
                }
            }
            return r;
        }

        public static TreatmentDTO CreateTreatmentDTO(ExcelWorksheet worksheet)
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
            var costs = LoadCosts(worksheet);
            var consequences = LoadConsequences(worksheet);
            var newTreatment = new TreatmentDTO
            {
                Name = worksheetName,
                Id = Guid.NewGuid(),
                Description = description,
                Category = treatmentCategory,
                AssetType = assetType,
                ShadowForAnyTreatment = ParseInt(yearsBeforeAny),
                ShadowForSameTreatment = ParseInt(yearsBeforeSame),
                Costs = costs,
                Consequences = consequences,
            };
            return newTreatment;
        }
    }
}
