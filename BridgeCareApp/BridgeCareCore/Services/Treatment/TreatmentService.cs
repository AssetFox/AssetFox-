using System;
using System.Collections.Generic;
using System.IO;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services.Treatment;
using BridgeCareCore.Utils;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class TreatmentService : ITreatmentService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentService(
            UnitOfDataPersistenceWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }
        public FileInfoDTO GenerateExcelFile(Guid libraryId)
        {
            var library = _unitOfWork.SelectableTreatmentRepo.GetTreatmentLibary(libraryId);
            var found = library != null;
            if (found)
            {
                var dateString = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
                var filename = $"Export treatment library {library.Name} {dateString}";
                var fileInfo = new FileInfo(filename);
                using var package = new ExcelPackage(fileInfo);
                var workbook = package.Workbook;
                TreatmentWorksheetGenerator.Fill(workbook, library);
                var bytes = package.GetAsByteArray();
                var fileData = Convert.ToBase64String(bytes);
                var r = new FileInfoDTO
                {
                    FileData = fileData,
                    FileName = filename,
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                };
                var hackFolder = @"C:\Users\WilliamJockusch\Desktop";
                var filePath = Path.Combine(hackFolder, filename);
                var filePathWithExtension = Path.ChangeExtension(filePath, ".xlsx");
                File.WriteAllBytes(filePathWithExtension, bytes);
                return r;
            }
            else
            {
                return null;
            }
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
            if (r == endIndex+1)
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

        public TreatmentImportResultDTO ImportLibraryTreatmentsFile(
            Guid treatmentLibraryId,
            ExcelPackage excelPackage)
        {
            var library = new TreatmentLibraryDTO
            {
                Treatments = new List<TreatmentDTO>(),
                Id = treatmentLibraryId,
            };
            foreach (var worksheet in excelPackage.Workbook.Worksheets)
            {
                var newTreatment = CreateTreatmentDTO(worksheet);
                library.Treatments.Add(newTreatment);
            }
            var r = new TreatmentImportResultDTO
            {
                TreatmentLibrary = library,
            };
            SaveToDatabase(r);
            return r;
        }

        private void SaveToDatabase(
            TreatmentImportResultDTO importResult)
        {
            var libraryId = importResult.TreatmentLibrary.Id;
            var importedTreatments = importResult.TreatmentLibrary.Treatments;
            _unitOfWork.SelectableTreatmentRepo.HandleImportCompletion(importedTreatments, libraryId);
        }

        private static List<TreatmentCostDTO> LoadCosts(ExcelWorksheet worksheet)
        {
            var r = new List<TreatmentCostDTO>();
            var costsLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Costs, 2);
            var consequencesLineIndex = FindRowWithFirstColumnContent(worksheet, TreatmentExportStringConstants.Consequences, costsLineIndex);
            for (var i=costsLineIndex+2; i<consequencesLineIndex; i++)
            {
                var equation = worksheet.Cells[i, 1].Text;
                var criterion = worksheet.Cells[i, 2].Text;
                if (!string.IsNullOrWhiteSpace(equation))
                {
                    var equationDto = new EquationDTO
                    {
                        Expression = equation,
                    };
                    CriterionLibraryDTO criterionLibrary = null;
                    if (criterion != null)
                    {
                        criterionLibrary = new CriterionLibraryDTO
                        {
                            MergedCriteriaExpression = criterion,
                            IsSingleUse = true,
                        };
                    }
                    var cost = new TreatmentCostDTO
                    {
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
            return r;
        }

        private TreatmentDTO CreateTreatmentDTO(ExcelWorksheet worksheet)
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
