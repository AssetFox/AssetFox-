using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using BridgeCareCore.Services.DefaultData;
using BridgeCareCoreTests.Helpers;
using OfficeOpenXml;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class InvestmentBudgetServiceIntegrationTests
    {
        private InvestmentBudgetsService CreateInvestmentBudgetsService(IUnitOfWork unitOfWork)
        {
            var investmentDefaultDataService = new InvestmentDefaultDataService();
            var expressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var hubService = HubServiceMocks.DefaultMock();
            var service = new InvestmentBudgetsService(
                unitOfWork,
                expressionValidationService.Object,
                hubService.Object,
                investmentDefaultDataService
                );
            return service;
        }

        [Fact]
        public void DownloadInvestmentLibrarySpreadsheet_ThenUpload_BudgetAmountUnchanged()
        {
            var budgetLibrary = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var budgetLibaryId = budgetLibrary.Id;
            var budgetId = Guid.NewGuid();
            var budget = BudgetTestSetup.AddBudgetToLibrary(TestHelper.UnitOfWork, budgetLibaryId, budgetId);
            var amountsBefore = TestHelper.UnitOfWork.BudgetAmountRepo.GetLibraryBudgetAmounts(budgetLibaryId);
            var investmentBudgetService = CreateInvestmentBudgetsService(TestHelper.UnitOfWork);
            var fileInfo = investmentBudgetService.ExportLibraryInvestmentBudgetsFile(budgetLibaryId);

            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            File.WriteAllBytes("zzzzz.xlsx", bytes);
            var excelPackage = new ExcelPackage(stream);
            var dictionary = new Dictionary<Guid, List<BudgetAmountDTO>>();
            TestHelper.UnitOfWork.BudgetAmountRepo.UpsertOrDeleteBudgetAmounts(dictionary, budgetLibaryId);
            var amountsIntermediate = TestHelper.UnitOfWork.BudgetAmountRepo.GetLibraryBudgetAmounts(budgetLibaryId);
            Assert.Empty(amountsIntermediate);
            var userCriteriaDto = new UserCriteriaDTO
            {
               
            };
            investmentBudgetService.ImportLibraryInvestmentBudgetsFile(budgetLibaryId, excelPackage, userCriteriaDto, true);
            var amountsAfter = TestHelper.UnitOfWork.BudgetAmountRepo.GetLibraryBudgetAmounts(budgetLibaryId);
            ObjectAssertions.Equivalent(amountsBefore, amountsAfter);
        }
    }
}
