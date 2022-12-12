using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using BridgeCareCore.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface IInvestmentBudgetsService
    {
        FileInfoDTO ExportScenarioInvestmentBudgetsFile(Guid simulationId);

        FileInfoDTO ExportLibraryInvestmentBudgetsFile(Guid budgetLibraryId);

        ScenarioBudgetImportResultDTO ImportScenarioInvestmentBudgetsFile(Guid simulationId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter,
            bool overwriteBudgets);

        BudgetImportResultDTO ImportLibraryInvestmentBudgetsFile(Guid budgetLibraryId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter,
            bool overwriteBudgets);
        List<BudgetDTO> GetNewLibraryDataset(InvestmentPagingSyncModel request);
    }
}
