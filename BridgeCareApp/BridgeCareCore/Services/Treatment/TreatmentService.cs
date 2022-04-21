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

        private Dictionary<string, string> DetailsSectionAsDictionary(ExcelWorksheet worksheet)
        {
            Dictionary<string, string> r = new Dictionary<string, string>();
            var costsRowIndex = 2;
            while (costsRowIndex < 100 && worksheet.Cells[costsRowIndex, 1].Text.ToLower() != TreatmentExportStringConstants.Costs.ToLower())
            {
                costsRowIndex++;
            }
            if (costsRowIndex == 100)
            {
                throw new Exception("Costs header cell not found!");
            }
            for (int i = 2; i < costsRowIndex; i++)
            {
                var name = worksheet.Cells[i, 1].Text.ToLower();
                var value = worksheet.Cells[i, 2].Text;
                r[name] = value;
            }
            return r;
        }

        private static int ParseInt(string s, int defaultValue = 0)
        {
            if (int.TryParse(s, out int r))
            {
                return r;
            }
            return 0;
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
            var costs = new List<TreatmentCostDTO>();
            var consequences = new List<TreatmentConsequenceDTO>();
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
