using System;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class PerformanceCurvesService : IPerformanceCurvesService
    {
        public PerformanceCurvesImportResultDTO ImportLibraryPerformanceCurvesFile(Guid budgetLibraryId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter)
        {
            return new PerformanceCurvesImportResultDTO();
        }

        public ScenarioPerformanceCurvesImportResultDTO ImportScenarioPerformanceCurvesFile(Guid simulationId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter)
        {
            return new ScenarioPerformanceCurvesImportResultDTO();
        }
    }
}
