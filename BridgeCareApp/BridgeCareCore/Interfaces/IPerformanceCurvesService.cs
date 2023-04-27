using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Common.Logging;
using System.Threading;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface IPerformanceCurvesService
    {
        ScenarioPerformanceCurvesImportResultDTO ImportScenarioPerformanceCurvesFile(Guid simulationId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null);

        PerformanceCurvesImportResultDTO ImportLibraryPerformanceCurvesFile(Guid performanceCurveLibraryId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null);

        FileInfoDTO ExportScenarioPerformanceCurvesFile(Guid simulationId);

        FileInfoDTO ExportLibraryPerformanceCurvesFile(Guid performanceCurveLibraryId);
        
    }
}
