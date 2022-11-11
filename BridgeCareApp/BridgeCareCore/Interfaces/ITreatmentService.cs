using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface ITreatmentService
    {
        FileInfoDTO ExportLibraryTreatmentsExcelFile(Guid libraryId);

        TreatmentImportResultDTO ImportLibraryTreatmentsFile(
            Guid treatmentLibraryId,
            ExcelPackage excelPackage);

        ScenarioTreatmentImportResultDTO ImportScenarioTreatmentsFile(
            Guid simulationId,
            ExcelPackage excelPackage);

        FileInfoDTO ExportScenarioTreatmentsExcelFile(Guid simulationId);

        TreatmentLibraryDTO GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO> upsertRequest);

        List<TreatmentDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<TreatmentDTO> request);
    }
}
