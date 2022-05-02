using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Services.Treatment;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class TreatmentService : ITreatmentService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly ExcelTreatmentLoader _treatmentLoader;

        public TreatmentService(
            UnitOfDataPersistenceWork unitOfWork,
            ExcelTreatmentLoader treatmentLoader
            )
        {
            _unitOfWork = unitOfWork;
            _treatmentLoader = treatmentLoader;
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

        private UserCriteriaDTO GetValidationCriteria(UserInfoDTO userInfo, string adminCheckConst)
        {
            var r = _unitOfWork.UserCriteriaRepo.GetOwnUserCriteria(userInfo, adminCheckConst);
            return r;
        }

        public TreatmentImportResultDTO ImportLibraryTreatmentsFile(
            Guid treatmentLibraryId,
            ExcelPackage excelPackage)
        {
            var validationMessages = new List<string>();
            var library = new TreatmentLibraryDTO
            {
                Treatments = new List<TreatmentDTO>(),
                Id = treatmentLibraryId,
            };
            foreach (var worksheet in excelPackage.Workbook.Worksheets)
            {
                var loadTreatment = _treatmentLoader.LoadTreatment(worksheet);
                library.Treatments.Add(loadTreatment.Treatment);
                validationMessages.AddRange(loadTreatment.ValidationMessages);
            }
            var combinedValidationMessage = "";
            if (validationMessages.Any())
            {
                var combinedValidationMessageBuilder = new StringBuilder();
                foreach (var message in validationMessages) {
                    combinedValidationMessageBuilder.AppendLine(message);
                }
                combinedValidationMessage = combinedValidationMessageBuilder.ToString();
            }
            var r = new TreatmentImportResultDTO
            {
                TreatmentLibrary = library,
                WarningMessage = combinedValidationMessage,
            };
            if (combinedValidationMessage.Length == 0)
            {
                SaveToDatabase(r);
            }
            return r;
        }

        private void SaveToDatabase(
            TreatmentImportResultDTO importResult)
        {
            var libraryId = importResult.TreatmentLibrary.Id;
            var importedTreatments = importResult.TreatmentLibrary.Treatments;
            _unitOfWork.SelectableTreatmentRepo.HandleImportCompletion(libraryId, importedTreatments);
        }
    }
}
