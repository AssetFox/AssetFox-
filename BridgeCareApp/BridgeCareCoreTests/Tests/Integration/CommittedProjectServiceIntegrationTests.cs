using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using OfficeOpenXml;
using Xunit;
using IamAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;


namespace BridgeCareCoreTests.Tests.Integration
{
    public class CommittedProjectServiceIntegrationTests
    {
        private CommittedProjectService CreateCommittedProjectService()
        {
            var service = new CommittedProjectService(TestHelper.UnitOfWork);
            return service;
        }

        [Fact]
        public void GetTreatmentCost_CriteriaFailToEvaluate_Throws()
        {
            var networkId = Guid.NewGuid();
            var service = CreateCommittedProjectService();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var assetKeyData = "key";
            var treatmentName = "treatment";
            var keyAttributeId = Guid.NewGuid();
            var maintainableAssets = new List<MaintainableAsset>();
            var assetId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var attributeName = RandomStrings.WithPrefix("attribute");
            var attribute = AttributeTestSetup.Text(keyAttributeId, attributeName);
            maintainableAssets.Add(maintainableAsset);
            var network = NetworkTestSetup.ModelForEntityInDbWithNewKeyTextAttribute(
                TestHelper.UnitOfWork, maintainableAssets, networkId, keyAttributeId, attributeName);
            var attributes = new List<IamAttribute> { attribute };
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, attributes, assetKeyData);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, "treatment");
            var treatmentCost = LibraryTreatmentCostTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentId, treatmentLibraryId, mergedCriteriaExpression: "ThrowingCriteria");

            var exception = Assert.Throws<CalculateEvaluateCompilationException>(() => service.GetTreatmentCost(
                treatmentLibraryId,
                assetKeyData,
                treatmentName,
                networkId));
            var expectedMessage = @"Unknown reference ""ThrowingCriteria"".";
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void GetTreatmentCost_CriteriaEvaluateToTrue_FindsCost()
        {
            var networkId = Guid.NewGuid();
            var service = CreateCommittedProjectService();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var assetKeyData = "key";
            var treatmentName = "treatment";
            var keyAttributeId = Guid.NewGuid();
            var maintainableAssets = new List<MaintainableAsset>();
            var assetId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var keyAttributeName = RandomStrings.WithPrefix("attribute");
            var keyAttribute = AttributeTestSetup.Text(keyAttributeId, keyAttributeName);
            var resultAttributeName = RandomStrings.WithPrefix("result");
            var resultAttributeId = Guid.NewGuid();
            var resultAttribute = AttributeTestSetup.Text(resultAttributeId, resultAttributeName); ;
            AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork,
                resultAttributeId, resultAttributeName, ConnectionType.EXCEL, keyAttributeName);
            maintainableAssets.Add(maintainableAsset);
            var network = NetworkTestSetup.ModelForEntityInDbWithNewKeyTextAttribute(
                TestHelper.UnitOfWork, maintainableAssets, networkId, keyAttributeId, keyAttributeName);
            var attributes = new List<IamAttribute> { keyAttribute, resultAttribute };
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, attributes, assetKeyData);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, "treatment");
            var treatmentCost = LibraryTreatmentCostTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentId, treatmentLibraryId, mergedCriteriaExpression: $"[{resultAttributeName}]='ok'");
            var keyAttributes = new List<IamAttribute> { keyAttribute };
            var resultAttributes = new List<IamAttribute> { resultAttribute };
            var resultDictionary = new Dictionary<string, List<IamAttribute>>();
            resultDictionary["ok"] = resultAttributes;
            resultDictionary["key"] = keyAttributes;
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, resultAttributes, "ok");
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, resultDictionary);

            var cost = service.GetTreatmentCost(
                treatmentLibraryId,
                assetKeyData,
                treatmentName,
                networkId);

            Assert.Equal(12345, cost);
        }

        [Fact]
        public void DownloadSpreadsheet_ThenReupload_Ok()
        {
            // failing as a part of a test run because MaintainableAssetDataRepository
            // caches KeyProperties.
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var assetKeyData = "key";
            var treatmentName = "treatment";
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
            var network = NetworkTestSetup.ModelForEntityInDbWithNewKeyTextAttribute(
                TestHelper.UnitOfWork, maintainableAssets, networkId, keyAttributeId, keyAttributeName);
            AdminSettingsTestSetup.SetupBamsAdminSettings(TestHelper.UnitOfWork, network.Name, keyAttributeName, keyAttributeName);
            var attributes = new List<IamAttribute> { keyAttribute, resultAttribute };
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, attributes, assetKeyData);

            var budgetLibraryId = Guid.NewGuid();
            var budgetLibraryName = RandomStrings.WithPrefix("BudgetLibrary ");
            var budgetLibrary = BudgetLibraryTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, budgetLibraryName, budgetLibraryId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetTestSetup.AddBudgetToLibrary(TestHelper.UnitOfWork, budgetLibraryId, budgetId);
            var scenarioBudgetId = Guid.NewGuid();
            budget.Id = scenarioBudgetId;

            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, treatmentName);
            var treatmentCost = LibraryTreatmentCostTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentId, treatmentLibraryId, mergedCriteriaExpression: $"[{resultAttributeName}]='ok'");

            var keyAttributes = new List<IamAttribute> { keyAttribute };
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            InvestmentPlanTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, null, 2023);
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(
               TestHelper.UnitOfWork, new List<BudgetDTO> { budget }, simulationId);

            var committedProjectId = Guid.NewGuid();
            var committedProject = SectionCommittedProjectDtos.Dto(
                committedProjectId,
                scenarioBudgetId,
                simulationId,
                ProjectSourceDTO.None,
                treatmentName,
                keyAttributeName,
                location.LocationIdentifier);
            committedProject.Year = 2023;
            committedProject.Cost = 31415926;
            committedProject.ShadowForAnyTreatment = 4;
            committedProject.ShadowForSameTreatment = 10;
            List<SectionCommittedProjectDTO> sectionCommittedProjects = new List<SectionCommittedProjectDTO> { committedProject };
            TestHelper.UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(sectionCommittedProjects);

            var committedProjects1 = TestHelper.UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            var committedProjectIds = new List<Guid> { committedProject.Id };
            var service = CreateCommittedProjectService();

            // first act
            var fileInfo = service.ExportCommittedProjectsFile(simulationId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            //File.WriteAllBytes("zzzzz.xlsx", bytes);
            var excelPackage = new ExcelPackage(stream);
            TestHelper.UnitOfWork.CommittedProjectRepo.DeleteSpecificCommittedProjects(committedProjectIds);
            var committedProjects2 = TestHelper.UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            Assert.Empty(committedProjects2);

            //second act
            service.ImportCommittedProjectFiles(simulationId, excelPackage, fileInfo.FileName);
            var committedProjects3 = TestHelper.UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            var id1 = committedProjects1[0].LocationKeys["ID"];
            var id3 = committedProjects3[0].LocationKeys["ID"];
            ObjectAssertions.EquivalentExcluding(committedProjects1, committedProjects3, x => x[0].LocationKeys, x => x[0].Id);
            Assert.NotEqual(id1, id3);
        }

        [Fact (Skip ="Conflict")]
        public void DownloadSpreadsheetWithTwoCommittedProjects_ThenReupload_Ok()
        {
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var assetKeyData = "key";
            var treatmentName = "treatment";
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
            var network = NetworkTestSetup.ModelForEntityInDbWithNewKeyTextAttribute(
                TestHelper.UnitOfWork, maintainableAssets, networkId, keyAttributeId, keyAttributeName);
            AdminSettingsTestSetup.SetupBamsAdminSettings(TestHelper.UnitOfWork, network.Name, keyAttributeName, keyAttributeName);
            var attributes = new List<IamAttribute> { keyAttribute, resultAttribute };
            AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
                maintainableAssets, attributes, assetKeyData);

            var budgetLibraryId = Guid.NewGuid();
            var budgetLibraryName = RandomStrings.WithPrefix("BudgetLibrary ");
            var budgetLibrary = BudgetLibraryTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, budgetLibraryName, budgetLibraryId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetTestSetup.AddBudgetToLibrary(TestHelper.UnitOfWork, budgetLibraryId, budgetId);
            var scenarioBudgetId = Guid.NewGuid();
            budget.Id = scenarioBudgetId;

            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, treatmentName);
            var treatmentCost = LibraryTreatmentCostTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentId, treatmentLibraryId, mergedCriteriaExpression: $"[{resultAttributeName}]='ok'");

            var keyAttributes = new List<IamAttribute> { keyAttribute };
            var simulationEntity = SimulationTestSetup.EntityInDb(TestHelper.UnitOfWork, networkId);
            var simulationId = simulationEntity.Id;
            InvestmentPlanTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, null, 2023);
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(
               TestHelper.UnitOfWork, new List<BudgetDTO> { budget }, simulationId);

            var committedProjectId1 = Guid.NewGuid();
            var committedProject1 = SectionCommittedProjectDtos.Dto(
                committedProjectId1,
                scenarioBudgetId,
                simulationId,
                ProjectSourceDTO.None,
                treatmentName,
                keyAttributeName,
                location.LocationIdentifier);
            committedProject1.Year = 2023;
            committedProject1.Cost = 31415926;
            committedProject1.ShadowForAnyTreatment = 1;
            committedProject1.ShadowForSameTreatment = 1;

            var committedProjectId2 = Guid.NewGuid();
            var committedProject2 = SectionCommittedProjectDtos.Dto(
                committedProjectId2,
                scenarioBudgetId,
                simulationId,
                ProjectSourceDTO.None,
                treatmentName,
                keyAttributeName,
                location.LocationIdentifier);
            committedProject2.Year = 2024;
            committedProject2.Cost = 12345678;
            committedProject2.ShadowForAnyTreatment = 1;
            committedProject2.ShadowForSameTreatment = 1;
            List<SectionCommittedProjectDTO> sectionCommittedProjects = new List<SectionCommittedProjectDTO> { committedProject1, committedProject2 };
            TestHelper.UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(sectionCommittedProjects);

            var committedProjects1 = TestHelper.UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            var committedProjectIds = new List<Guid> { committedProjectId1, committedProjectId2 };
            var service = CreateCommittedProjectService();

            // first act
            var fileInfo = service.ExportCommittedProjectsFile(simulationId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
         //   File.WriteAllBytes("zzzzz.xlsx", bytes);
            var excelPackage = new ExcelPackage(stream);
            TestHelper.UnitOfWork.CommittedProjectRepo.DeleteSpecificCommittedProjects(committedProjectIds);
            var committedProjects2 = TestHelper.UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            Assert.Empty(committedProjects2);

            //second act
            service.ImportCommittedProjectFiles(simulationId, excelPackage, fileInfo.FileName);
            var committedProjects3 = TestHelper.UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            var id1 = committedProjects1[0].LocationKeys["ID"];
            var id3 = committedProjects3[0].LocationKeys["ID"];
            ObjectAssertions.EquivalentExcluding(committedProjects1, committedProjects3, x => x[0].LocationKeys, x => x[0].Id, x => x[1].LocationKeys, x => x[1].Id);
            Assert.NotEqual(id1, id3);
        }

        [Fact]
        public void DownloadTemplate_IsValidExcelPackage()
        {
            var networkId = Guid.NewGuid();
            var keyAttributeId = Guid.NewGuid();
            var maintainableAssets = new List<MaintainableAsset>();
            var assetId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var keyAttributeName = RandomStrings.WithPrefix("locationAttribute");
            var keyAttribute = AttributeTestSetup.Text(keyAttributeId, keyAttributeName, ConnectionType.EXCEL);
            maintainableAssets.Add(maintainableAsset);
            var network = NetworkTestSetup.ModelForEntityInDbWithNewKeyTextAttribute(
                TestHelper.UnitOfWork, maintainableAssets, networkId, keyAttributeId, keyAttributeName);
            AdminSettingsTestSetup.SetupBamsAdminSettings(TestHelper.UnitOfWork, network.Name, keyAttributeName, keyAttributeName);
            var service = CreateCommittedProjectService();

            var fileInfo = service.CreateCommittedProjectTemplate(network.Id);

            ExcelPackageAsserts.ValidExcelPackageData(fileInfo);
        }
    }
}
