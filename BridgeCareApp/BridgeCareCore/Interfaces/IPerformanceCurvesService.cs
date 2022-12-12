using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface IPerformanceCurvesService
    {
        ScenarioPerformanceCurvesImportResultDTO ImportScenarioPerformanceCurvesFile(Guid simulationId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter);

        PerformanceCurvesImportResultDTO ImportLibraryPerformanceCurvesFile(Guid performanceCurveLibraryId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter);

        FileInfoDTO ExportScenarioPerformanceCurvesFile(Guid simulationId);

        FileInfoDTO ExportLibraryPerformanceCurvesFile(Guid performanceCurveLibraryId);

        PagingPageModel<PerformanceCurveDTO> GetScenarioPerformanceCurvePage(Guid simulationId, PagingRequestModel<PerformanceCurveDTO> request);
        PagingPageModel<PerformanceCurveDTO> GetLibraryPerformanceCurvePage(Guid libraryId, PagingRequestModel<PerformanceCurveDTO> request);
        List<PerformanceCurveDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<PerformanceCurveDTO> request);
        List<PerformanceCurveDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<PerformanceCurveDTO> request);

        List<PerformanceCurveDTO> GetNewLibraryDataset(PagingSyncModel<PerformanceCurveDTO> pagingSync);
    }
}
