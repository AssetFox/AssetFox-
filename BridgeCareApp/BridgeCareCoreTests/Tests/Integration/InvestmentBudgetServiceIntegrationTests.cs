using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using BridgeCareCore.Services.DefaultData;
using BridgeCareCoreTests.Helpers;
using OfficeOpenXml;
using Xunit;
using IamAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using AppliedResearchAssociates.iAM.Data;
using System.Collections.Immutable;

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
            var _ = BudgetTestSetup.AddBudgetToLibrary(TestHelper.UnitOfWork, budgetLibaryId, budgetId);
            var amountsBefore = TestHelper.UnitOfWork.BudgetAmountRepo.GetLibraryBudgetAmounts(budgetLibaryId);
            var investmentBudgetService = CreateInvestmentBudgetsService(TestHelper.UnitOfWork);
            var fileInfo = investmentBudgetService.ExportLibraryInvestmentBudgetsFile(budgetLibaryId);

            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            //File.WriteAllBytes("zzzzz.xlsx", bytes);
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

        [Fact]
        public void DownloadScenarioBudgetAmountSpreadsheet_ThenUpload_SameAmounts()
        {
            var networkId = Guid.NewGuid();
            var assetKeyData = "key";
            var keyAttributeId = Guid.NewGuid();
            var maintainableAssets = new List<MaintainableAsset>();
            var assetId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var keyAttributeName = RandomStrings.WithPrefix("locationAttribute");
            var keyAttribute = AttributeTestSetup.Text(keyAttributeId, keyAttributeName, ConnectionType.EXCEL);
            var resultAttributeName = RandomStrings.WithPrefix("result");
            var resultAttributeId = Guid.NewGuid();
            var resultAttribute = AttributeTestSetup.Text(resultAttributeId, resultAttributeName, ConnectionType.EXCEL);
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork,
                resultAttributeId, resultAttributeName, ConnectionType.EXCEL, keyAttributeName);
            maintainableAssets.Add(maintainableAsset);
            var network = NetworkTestSetup.ModelForEntityInDbWithKeyAttribute(
                TestHelper.UnitOfWork, maintainableAssets, networkId, keyAttributeId, keyAttributeName);
            AdminSettingsTestSetup.SetupBamsAdminSettings(TestHelper.UnitOfWork, network.Name, keyAttributeName, keyAttributeName);
            var attributes = new List<IamAttribute> { keyAttribute, resultAttribute };
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, attributes, assetKeyData);
            var keyAttributes = new List<IamAttribute> { keyAttribute };
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.WithSingleAmount(budgetId, "budget", 2023, 123456);
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);
            var investmentBudgetService = CreateInvestmentBudgetsService(TestHelper.UnitOfWork);
            var fileInfo = investmentBudgetService.ExportScenarioInvestmentBudgetsFile(simulationId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            //File.WriteAllBytes("zzzzz.xlsx", bytes);
            var budgets1 = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, new List<BudgetDTO>(), simulationId);
            var budgets2 = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
            Assert.Empty(budgets2);
            var excelPackage = new ExcelPackage(stream);
            var userCriteria = new UserCriteriaDTO();
            investmentBudgetService.ImportScenarioInvestmentBudgetsFile(simulationId, excelPackage, userCriteria, true);
            var budgets3 = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
            ObjectAssertions.EquivalentExcluding(budgets1, budgets3, b => b[0].Id, b => b[0].BudgetAmounts[0].Id);
        }
    }
}
