using System;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface IInvestmentBudgetsService
    {
        FileInfoDTO ExportInvestmentBudgetsFile(Guid budgetLibraryId);

        BudgetLibraryDTO ImportInvestmentBudgetsFile(Guid budgetLibraryId, ExcelPackage excelPackage);
    }
}
