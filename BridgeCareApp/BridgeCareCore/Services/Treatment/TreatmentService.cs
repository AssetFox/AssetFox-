using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;using System.Threading;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
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

        public FileInfoDTO ExportLibraryTreatmentsExcelFile(Guid libraryId)
        {
            var library = _unitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(libraryId);
            var found = library != null;
            if (found)
            {
                var dateString = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
                var filename = $"Export treatment library {library.Name} {dateString}";
                var fileInfo = new FileInfo(filename);
                using var package = new ExcelPackage(fileInfo);
                var workbook = package.Workbook;
                TreatmentWorksheetGenerator.Fill(workbook, library.Treatments);
                var bytes = package.GetAsByteArray();
                var fileData = Convert.ToBase64String(bytes);
                var returnValue = new FileInfoDTO
                {
                    FileData = fileData,
                    FileName = filename,
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                };
                return returnValue;
            }
            else
            {
                return null;
            }
        }

        public void ImportScenarioTreatmentsFileSingle(Guid simulationId, ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null)
        {
            queueLog ??= new DoNothingWorkQueueLog();
            var validationMessages = new List<string>();
            var scenarioTreatments = new List<TreatmentDTO>();
            var scenarioBudgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
            queueLog.UpdateWorkQueueStatus("Loading Excel");
            foreach (var worksheet in excelPackage.Workbook.Worksheets)
            {
                var treatmentLoadResult = _treatmentLoader.LoadScenarioTreatment(worksheet, scenarioBudgets);
                scenarioTreatments.Add(treatmentLoadResult.Treatment);
                validationMessages.AddRange(treatmentLoadResult.ValidationMessages);
            }
            var combinedValidationMessage = string.Empty;
            if (validationMessages.Any())
            {
                var combinedValidationMessageBuilder = new StringBuilder();
                foreach (var message in validationMessages)
                {
                    combinedValidationMessageBuilder.AppendLine(message);
                }
                combinedValidationMessage = combinedValidationMessageBuilder.ToString();
            }

            var scenarioTreatmentImportResult = new ScenarioTreatmentImportResultDTO
            {
                Treatments = scenarioTreatments,
                WarningMessage = combinedValidationMessage,
            };
            if (combinedValidationMessage.Length == 0)
            {
                queueLog.UpdateWorkQueueStatus("Upserting Treatments");
                _unitOfWork.SelectableTreatmentRepo.AddScenarioSelectableTreatment(scenarioTreatmentImportResult.Treatments, simulationId);
            }
        }

        public void ImportLibraryTreatmentsFileSingle(
            Guid treatmentLibraryId,
            ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null)
        {
            queueLog ??= new DoNothingWorkQueueLog();
            queueLog.UpdateWorkQueueStatus("Starting Import");
            var validationMessages = new List<string>();            //if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                //return new TreatmentImportResultDTO();
            var treatmentLibrary = _unitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(treatmentLibraryId);
            var library = new TreatmentLibraryDTO
            {
                Treatments = new List<TreatmentDTO>(),
                Id = treatmentLibraryId,
                Name = treatmentLibrary.Name,
                Owner = treatmentLibrary.Owner,
                Description = treatmentLibrary.Description
            };
            foreach (var worksheet in excelPackage.Workbook.Worksheets)
            {
                //if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                    //return new TreatmentImportResultDTO();
                var loadTreatment = _treatmentLoader.LoadTreatment(worksheet);
                library.Treatments.Add(loadTreatment.Treatment);
                validationMessages.AddRange(loadTreatment.ValidationMessages);
            }

            _unitOfWork.SelectableTreatmentRepo.AddLibraryTreatments(library.Treatments, library.Id);
      
        }
        public TreatmentImportResultDTO ImportLibraryTreatmentsFile(
            Guid treatmentLibraryId,
            ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null)
        {
            queueLog ??= new DoNothingWorkQueueLog();
            queueLog.UpdateWorkQueueStatus("Starting Import");
            var validationMessages = new List<string>();            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                return new TreatmentImportResultDTO();
            var treatmentLibrary = _unitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(treatmentLibraryId);
            var library = new TreatmentLibraryDTO
            {
                Treatments = new List<TreatmentDTO>(),
                Id = treatmentLibraryId,
                Name = treatmentLibrary.Name,
                Owner = treatmentLibrary.Owner,
                Description = treatmentLibrary.Description
            };
            foreach (var worksheet in excelPackage.Workbook.Worksheets)
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                    return new TreatmentImportResultDTO();
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
            var returnValue = new TreatmentImportResultDTO
            {
                TreatmentLibrary = library,
                WarningMessage = combinedValidationMessage,
            };            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                return new TreatmentImportResultDTO();
            if (combinedValidationMessage.Length == 0)
            {
                queueLog.UpdateWorkQueueStatus("Updating Treatment Library");
                SaveToDatabase(returnValue);
            }
            return returnValue;
        }
        public ScenarioTreatmentImportResultDTO ImportScenarioTreatmentsFile(Guid simulationId, ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null)
        {
            queueLog ??= new DoNothingWorkQueueLog();
            var validationMessages = new List<string>();
            var scenarioTreatments = new List<TreatmentDTO>();
            var scenarioBudgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                return new ScenarioTreatmentImportResultDTO();
            queueLog.UpdateWorkQueueStatus("Loading Excel");
            foreach (var worksheet in excelPackage.Workbook.Worksheets)
            {                
                var treatmentLoadResult = _treatmentLoader.LoadScenarioTreatment(worksheet, scenarioBudgets);
                scenarioTreatments.Add(treatmentLoadResult.Treatment);
                validationMessages.AddRange(treatmentLoadResult.ValidationMessages);
            }
            var combinedValidationMessage = string.Empty;
            if (validationMessages.Any())
            {
                var combinedValidationMessageBuilder = new StringBuilder();
                foreach (var message in validationMessages)
                {
                    combinedValidationMessageBuilder.AppendLine(message);
                }
                combinedValidationMessage = combinedValidationMessageBuilder.ToString();
            }

            var scenarioTreatmentImportResult = new ScenarioTreatmentImportResultDTO
            {
                Treatments = scenarioTreatments,
                WarningMessage = combinedValidationMessage,
            };
            if (combinedValidationMessage.Length == 0)
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                    return new ScenarioTreatmentImportResultDTO();
                queueLog.UpdateWorkQueueStatus("Upserting Treatments");
                _unitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(scenarioTreatmentImportResult.Treatments, simulationId);
            }
            return scenarioTreatmentImportResult;
        }
        public FileInfoDTO ExportScenarioTreatmentsExcelFile(Guid simulationId)
        {
            var fileInfoResult = new FileInfoDTO();
            var scenarioName = _unitOfWork.SimulationRepo.GetSimulationName(simulationId);
            var scenarioTreatments = _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
            if (scenarioTreatments.Any())
            {
                var dateString = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
                var filename = $"Export scenario {scenarioName} treatments {dateString}";
                var fileInfo = new FileInfo(filename);
                using var package = new ExcelPackage(fileInfo);
                var workbook = package.Workbook;
                TreatmentWorksheetGenerator.Fill(workbook, scenarioTreatments);
                var bytes = package.GetAsByteArray();
                var fileData = Convert.ToBase64String(bytes);
                fileInfoResult = new FileInfoDTO
                {
                    FileData = fileData,
                    FileName = filename,
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                };
            }
            return fileInfoResult;
        }

        public TreatmentSupersedeRuleImportResultDTO ImportScenarioTreatmentSupersedeRulesFile(Guid simulationId, ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null)
        {
            queueLog ??= new DoNothingWorkQueueLog();            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
            {
                return new TreatmentSupersedeRuleImportResultDTO();
            }
            queueLog.UpdateWorkQueueStatus("Loading Excel");

            var worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
            var scenarioTreatments = _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
            var treatmentSupersedeRuleResult = _treatmentLoader.LoadTreatmentSupersedeRules(worksheet, scenarioTreatments);
            var scenarioTreatmentSupersedeRuleImportResult = GetTreatmentSupersedeRuleImportResultDTO(treatmentSupersedeRuleResult);

            if (scenarioTreatmentSupersedeRuleImportResult.WarningMessage.Length == 0)
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                {
                    return new TreatmentSupersedeRuleImportResultDTO();
                }
                queueLog.UpdateWorkQueueStatus("Upserting Scenario Treatment Supersede Rules");
                _unitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteScenarioTreatmentSupersedeRules(scenarioTreatmentSupersedeRuleImportResult.supersedeRulesPerTreatmentIdDict, simulationId);
            }
            return scenarioTreatmentSupersedeRuleImportResult;
        }

        public FileInfoDTO ExportScenarioTreatmentSupersedeRuleExcelFile(Guid simulationId)
        {
            var simulation = _unitOfWork.SimulationRepo.GetSimulation(simulationId);            
            var scenarioTreatmentSupersedeRules = _unitOfWork.TreatmentSupersedeRuleRepo.GetScenarioTreatmentSupersedeRulesBysimulationId(simulationId); 
            var fileName = $"ScenarioTreatmentSupersedeRules_{simulation.Name.Trim().Replace(" ", "_")}.xlsx";

            return CreateExportScenarioTreatmentRuleExportFile(scenarioTreatmentSupersedeRules, fileName);
        }

        public FileInfoDTO ExportLibraryTreatmentSupersedeRuleExcelFile(Guid libraryId)
        {
            var library = _unitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibaryNoChildren(libraryId) ?? throw new NullReferenceException("No Treatment Library found for given id");
            var libraryTreatmentSupersedeRules = _unitOfWork.TreatmentSupersedeRuleRepo.GetLibraryTreatmentSupersedeRulesByLibraryId(libraryId);
            var fileName = $"TreatmentSupersedeRules_{library.Name.Trim().Replace(" ", "_")}.xlsx";

            return CreateExportScenarioTreatmentRuleExportFile(libraryTreatmentSupersedeRules, fileName);
        }

        public TreatmentSupersedeRuleImportResultDTO ImportLibraryTreatmentSupersedeRulesFile(Guid libraryId, ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null)
        {
            queueLog ??= new DoNothingWorkQueueLog();            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
            {
                return new TreatmentSupersedeRuleImportResultDTO();
            }
            queueLog.UpdateWorkQueueStatus("Loading Excel");

            var worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
            var libraryTreatments = _unitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(libraryId);
            var treatmentSupersedeRuleResult = _treatmentLoader.LoadTreatmentSupersedeRules(worksheet, libraryTreatments);
            var treatmentSupersedeRuleImportResult = GetTreatmentSupersedeRuleImportResultDTO(treatmentSupersedeRuleResult);

            if (treatmentSupersedeRuleImportResult.WarningMessage.Length == 0)
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                {
                    return new TreatmentSupersedeRuleImportResultDTO();
                }
                queueLog.UpdateWorkQueueStatus("Upserting Scenario Treatment Supersede Rules");
                _unitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteTreatmentSupersedeRules(treatmentSupersedeRuleImportResult.supersedeRulesPerTreatmentIdDict, libraryId);
            }
            return treatmentSupersedeRuleImportResult;
        }

        private static TreatmentSupersedeRuleImportResultDTO GetTreatmentSupersedeRuleImportResultDTO(TreatmentSupersedeRulesLoadResult treatmentSupersedeRuleResult)
        {
            var combinedValidationMessage = string.Empty;
            var validationMessages = treatmentSupersedeRuleResult.ValidationMessages;
            if (validationMessages.Any())
            {
                var combinedValidationMessageBuilder = new StringBuilder();
                foreach (var message in validationMessages)
                {
                    combinedValidationMessageBuilder.AppendLine(message);
                }
                combinedValidationMessage = combinedValidationMessageBuilder.ToString();
            }

            var scenarioTreatmentSupersedeRuleImportResult = new TreatmentSupersedeRuleImportResultDTO
            {
                supersedeRulesPerTreatmentIdDict = treatmentSupersedeRuleResult.supersedeRulesPerTreatmentIdDict,
                WarningMessage = combinedValidationMessage,
            };

            return scenarioTreatmentSupersedeRuleImportResult;
        }

        private static FileInfoDTO CreateExportScenarioTreatmentRuleExportFile(List<TreatmentSupersedeRuleExportDTO> treatmentSupersedeRules, string fileName)
        {
            using var excelPackage = new ExcelPackage(new FileInfo(fileName));
            var worksheet = excelPackage.Workbook.Worksheets.Add("Treatment Supersede Rules");

            // headers
            var startRow = worksheet.Cells.Start.Row;
            var startColumn = worksheet.Cells.Start.Column;
            var headerColumn = startColumn;
            worksheet.Cells[startRow, headerColumn++].Value = "Treatment Name (selected treatment)";
            worksheet.Cells[startRow, headerColumn++].Value = "Superseded treatment";
            worksheet.Cells[startRow, headerColumn++].Value = "Criteria";

            // data rows
            var dataRow = startRow + 1;
            foreach (var treatmentSupersedeRule in treatmentSupersedeRules)
            {
                var dataColumn = startColumn;
                worksheet.Cells[dataRow, dataColumn++].Value = treatmentSupersedeRule.TreatmentName;
                worksheet.Cells[dataRow, dataColumn++].Value = treatmentSupersedeRule.treatment.Name;
                worksheet.Cells[dataRow, dataColumn++].Value = treatmentSupersedeRule.CriterionLibrary?.MergedCriteriaExpression;
                dataRow++;
            }
            worksheet.Cells.AutoFitColumns();

            return new FileInfoDTO
            {
                FileName = fileName,
                FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        private void SaveToDatabase(
            TreatmentImportResultDTO importResult)
        {
            var libraryId = importResult.TreatmentLibrary.Id;
            var importedTreatments = importResult.TreatmentLibrary.Treatments;
            _unitOfWork.SelectableTreatmentRepo.ReplaceTreatmentLibrary(libraryId, importedTreatments);
        }        
    }
}
