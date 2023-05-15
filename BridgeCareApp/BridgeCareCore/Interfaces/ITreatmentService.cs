using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Common.Logging;
using System.Threading;
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
            ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null);

        ScenarioTreatmentImportResultDTO ImportScenarioTreatmentsFile(
            Guid simulationId,
            ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null);

        FileInfoDTO ExportScenarioTreatmentsExcelFile(Guid simulationId);
    }
}
