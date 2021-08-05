using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface IInvestmentBudgetsService
    {
        FileInfoDTO ExportScenarioInvestmentBudgetsFile(Guid simulationId);

        FileInfoDTO ExportLibraryInvestmentBudgetsFile(Guid budgetLibraryId);

        List<BudgetDTO> ImportScenarioInvestmentBudgetsFile(Guid simulationId, ExcelPackage excelPackage);

        BudgetLibraryDTO ImportLibraryInvestmentBudgetsFile(Guid budgetLibraryId, ExcelPackage excelPackage);
    }
}
