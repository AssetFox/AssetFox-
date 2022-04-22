using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IExpressionValidationService _expressionValidationService;

        public TreatmentService(
            UnitOfDataPersistenceWork unitOfWork,
            IExpressionValidationService expressionValidationService
            )
        {
            _unitOfWork = unitOfWork;
            _expressionValidationService = expressionValidationService;
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

        public object Validate(TreatmentDTO treatment)
        {
            var criteria = new UserCriteriaDTO();
            foreach (var cost in treatment.Costs) {
                var equationValidationParameters = new EquationValidationParameters
                {
                    Expression = cost.Equation.Expression,
                    CurrentUserCriteriaFilter = criteria,
                    IsPiecewise = false,
                };
                var validationResult = _expressionValidationService.ValidateEquation(equationValidationParameters);

            }
            return null;
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
                var newTreatment = ExcelTreatmentLoader.CreateTreatmentDTO(worksheet);
                var validation = Validate(newTreatment);
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
            _unitOfWork.SelectableTreatmentRepo.HandleImportCompletion(libraryId, importedTreatments);
        }
    }
}
