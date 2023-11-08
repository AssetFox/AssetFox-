using System;
using AppliedResearchAssociates.iAM.Common.Logging;
using System.Threading;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface ITreatmentService
    {
        FileInfoDTO ExportLibraryTreatmentsExcelFile(Guid libraryId);

        TreatmentImportResultDTO ImportLibraryTreatmentsFile(
            Guid treatmentLibraryId,
            ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null);

        void ImportLibraryTreatmentsFileSingle(
            Guid treatmentLibraryId,
            ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null);

        void ImportScenarioTreatmentsFileSingle(
           Guid treatmentLibraryId,
           ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null);

        ScenarioTreatmentImportResultDTO ImportScenarioTreatmentsFile(
            Guid simulationId,
            ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null);

        FileInfoDTO ExportScenarioTreatmentsExcelFile(Guid simulationId);

        FileInfoDTO ExportScenarioTreatmentSupersedeRuleExcelFile(Guid simulationId);

        ScenarioTreatmentSupersedeRuleImportResultDTO ImportScenarioTreatmentSupersedeRulesFile(Guid simulationId, ExcelPackage excelPackage, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null);
    }
}
