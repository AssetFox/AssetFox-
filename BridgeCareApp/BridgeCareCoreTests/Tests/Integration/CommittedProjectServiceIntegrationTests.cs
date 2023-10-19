using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
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
            var network = NetworkTestSetup.ModelForEntityInDbWithKeyAttribute(
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
            var network = NetworkTestSetup.ModelForEntityInDbWithKeyAttribute(
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
        //[Fact (Skip ="Resurrect while fixing committed project spreadsheet upload")]
        public void DownloadSpreadsheet_ThenReupload_Ok()
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
            var network = NetworkTestSetup.ModelForEntityInDbWithKeyAttribute(
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
            var committedProjectsBefore = TestHelper.UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            var service = CreateCommittedProjectService();
            var fileInfo = service.ExportCommittedProjectsFile(simulationId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
       //     File.WriteAllBytes("zzzzz.xlsx", bytes);
            var excelPackage = new ExcelPackage(stream);
            service.ImportCommittedProjectFiles(simulationId, excelPackage, fileInfo.FileName, true);
            var committedProjectsAfter = TestHelper.UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            var id1 = committedProjectsBefore[0].LocationKeys["ID"];
            var id2 = committedProjectsAfter[0].LocationKeys["ID"];
            ObjectAssertions.EquivalentExcluding(committedProjectsBefore, committedProjectsAfter, x => x[0].LocationKeys, x => x[0].Id);
        }
    }
}
