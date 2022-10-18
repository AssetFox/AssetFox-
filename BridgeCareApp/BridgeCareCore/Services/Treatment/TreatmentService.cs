using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Services.Treatment;
using Microsoft.SqlServer.Dac.Model;
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

        public TreatmentImportResultDTO ImportLibraryTreatmentsFile(
            Guid treatmentLibraryId,
            ExcelPackage excelPackage)
        {
            var validationMessages = new List<string>();
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
            };
            if (combinedValidationMessage.Length == 0)
            {
                SaveToDatabase(returnValue);
            }
            return returnValue;
        }

        public ScenarioTreatmentImportResultDTO ImportScenarioTreatmentsFile(Guid simulationId, ExcelPackage excelPackage)
        {
            var validationMessages = new List<string>();
            var scenarioTreatments = new List<TreatmentDTO>();

            var scenarioBudgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
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

        private void SaveToDatabase(
            TreatmentImportResultDTO importResult)
        {
            var libraryId = importResult.TreatmentLibrary.Id;
            var importedTreatments = importResult.TreatmentLibrary.Treatments;
            _unitOfWork.SelectableTreatmentRepo.ReplaceTreatmentLibrary(libraryId, importedTreatments);
        }

        public TreatmentLibraryDTO GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO> upsertRequest)
        {
            Guid? libraryId = null;

            var rows = new List<TreatmentDTO>();
            if (upsertRequest.PagingSync.LibraryId != null)
                libraryId = upsertRequest.PagingSync.LibraryId.Value;
            else if (!upsertRequest.IsNewLibrary)
                libraryId = upsertRequest.Library.Id;

            if(libraryId != null)
                rows = _unitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(libraryId.Value);
            rows = SyncedDataset(rows, upsertRequest.PagingSync);

            if (upsertRequest.PagingSync.LibraryId != null && upsertRequest.PagingSync.LibraryId != upsertRequest.Library.Id)
            {
                rows.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.CriterionLibrary.Id = Guid.NewGuid();
                    _.Consequences.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        __.CriterionLibrary.Id = Guid.NewGuid();
                        __.Equation.Id = Guid.NewGuid();
                    });
                    _.Costs.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        __.Equation.Id = Guid.NewGuid();
                        __.CriterionLibrary.Id = Guid.NewGuid();
                    });
                });
            }
            var dto = upsertRequest.Library;
            dto.Treatments = rows;

            return dto;
        }
        public List<TreatmentDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<TreatmentDTO> request)
        {
            var rows = request.LibraryId == null ?
                    _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId) :
                    _unitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(request.LibraryId.Value);
            rows = SyncedDataset(rows, request);

            if (request.LibraryId != null)
                rows.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.CriterionLibrary.Id = Guid.NewGuid();
                    _.Consequences.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        __.CriterionLibrary.Id = Guid.NewGuid();
                        __.Equation.Id = Guid.NewGuid();
                    });
                    _.Costs.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        __.Equation.Id = Guid.NewGuid();
                        __.CriterionLibrary.Id = Guid.NewGuid();
                    });                   
                });
            return rows;
        }

        private List<TreatmentDTO> SyncedDataset(List<TreatmentDTO> rows, PagingSyncModel<TreatmentDTO> request)
        {
            rows = rows.Concat(request.AddedRows).Where(_ => !request.RowsForDeletion.Contains(_.Id)).ToList();

            for (var i = 0; i < rows.Count; i++)
            {
                var item = request.UpdateRows.FirstOrDefault(row => row.Id == rows[i].Id);
                if (item != null)
                    rows[i] = item;
            }

            return rows;
        }
    }
}
