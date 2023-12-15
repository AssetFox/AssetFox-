using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;
using IamAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class OrphanCleanupSprocTests
    {
        private string RunOrphanCleanupSproc()
        {
            var retMessageParam = new SqlParameter("@RetMessage", SqlDbType.VarChar, 250);
            retMessageParam.Direction = ParameterDirection.Output;
            TestHelper.UnitOfWork.Context.Database.ExecuteSqlRaw("EXEC usp_orphan_cleanup @RetMessage", retMessageParam);
            var retMessageValue = retMessageParam.Value;
            return retMessageValue.ToString();
        }

        [Fact]
        public void OrphanCleanup_Runs()
        {
            var retMessage = RunOrphanCleanupSproc();
            Assert.Empty(retMessage);
        }

        [Fact]
        public void RunOrphanCleanup_OrphanInDb_Deletes()
        {
            var orphanId = Guid.NewGuid();
            var nonexistentNetworkId = Guid.NewGuid();
            var orphan = new MaintainableAssetEntity
            {
                Id = orphanId,
                AssetName = "orphan",
                NetworkId = nonexistentNetworkId,
            };
            var entities = new List<MaintainableAssetEntity> { orphan };
            TestHelper.UnitOfWork.Context.AddAll(entities);
            var orphanInDbBefore = TestHelper.UnitOfWork.Context.MaintainableAsset
                .SingleOrDefault(a => a.Id == orphanId);
            Assert.NotNull(orphanInDbBefore);
            RunOrphanCleanupSproc();
            var orphanInDbAfter = TestHelper.UnitOfWork.Context.MaintainableAsset
             .SingleOrDefault(a => a.Id == orphanId);
            Assert.Null(orphanInDbAfter);
        }

        [Fact]
        public void RunOrphanCleanup_NonOrphanInDb_DoesNotDelete()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var keyAttributeId = Guid.NewGuid();
            var keyAttributeName = RandomStrings.WithPrefix("KeyAttribute");
            var attributeDto = AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, keyAttributeId, keyAttributeName, ConnectionType.EXCEL, "location");
            var asset = MaintainableAssets.InNetwork(networkId, keyAttributeName, assetId);
            var assets = new List<MaintainableAsset> { asset };
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, assets, networkId, keyAttributeId);
            var entityInDbBefore = TestHelper.UnitOfWork.Context.MaintainableAsset
                .SingleOrDefault(a => a.Id == assetId);
            Assert.NotNull(entityInDbBefore);
            RunOrphanCleanupSproc();
            var entityInDbAfter = TestHelper.UnitOfWork.Context.MaintainableAsset
             .SingleOrDefault(a => a.Id == assetId);
            Assert.NotNull(entityInDbAfter);
        }

        [Fact]
        public async Task RunOrphanCleanupSproc_DataInDbFromTestSetups_DeletesNothing()
        {
            RunOrphanCleanupSproc();
            var dataSource = AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var networkEntity = NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var allAttributes = TestHelper.UnitOfWork.AttributeRepo.GetAttributes();
            var keyAttribute = allAttributes.Single(a => a.Id == networkEntity.KeyAttributeId);
            AdminSettingsTestSetup.SetupBamsAdminSettings(TestHelper.UnitOfWork, NetworkTestSetup.TestNetwork().Name, keyAttribute.Name, keyAttribute.Name);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var userId = user.Id;
            var networkId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var asset = MaintainableAssets.InNetwork(networkId, keyAttribute.Name, assetId);
            var assets = new List<MaintainableAsset> { asset };
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, assets, networkId, keyAttribute.Id);
            var attribute = AttributeDtos.DeckDurationN;
            var numericAttribute = AttributeTestSetup.Numeric(attribute.Id, attribute.Name, dataSource.Id);
            var numericAttributeList = new List<IamAttribute> { numericAttribute };
            AggregatedResultTestSetup.AddNumericAggregatedResultsToDb(TestHelper.UnitOfWork, assets, numericAttributeList);
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var simulationTreatment = TreatmentTestSetup.ModelForSingleTreatmentOfSimulationInDb(TestHelper.UnitOfWork, simulationId);
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var libraryTreatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var budgetLibraryId = Guid.NewGuid();
            var budgetLibrary = BudgetLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, "budgetLibraryNameHere", budgetLibraryId);
            var budgetId = Guid.NewGuid();
            var budget = BudgetTestSetup.AddBudgetToLibrary(TestHelper.UnitOfWork, budgetLibraryId, budgetId);
            var scenarioBudgetId = Guid.NewGuid();
            var scenarioBudgetName = RandomStrings.WithPrefix("ScenarioBudget");
            var scenarioBudget = BudgetDtos.WithSingleAmount(budgetId, scenarioBudgetName, 2023, 100.1m);
            var scenarioBudgets = new List<BudgetDTO> { scenarioBudget };
            var performanceCurveLibraryId = Guid.NewGuid();
            var performanceCurveLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, performanceCurveLibraryId);
            var libraryPerformanceCurveId = Guid.NewGuid();
            var libraryPerformanceCurve = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, performanceCurveLibraryId, libraryPerformanceCurveId, TestAttributeNames.CulvDurationN);
            var scenarioPerformanceId = Guid.NewGuid();
            var scenarioPerformanceCurveCriterionLibrary = CriterionLibraryDtos.Dto();
            var simulationPerformanceCurve = ScenarioPerformanceCurveTestSetup.DtoForEntityInDb(TestHelper.UnitOfWork, simulationId, scenarioPerformanceId, scenarioPerformanceCurveCriterionLibrary, "pretendEquation");
            
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, scenarioBudgets, simulationId);

            var rowCounts1 = TableRowCounter.CountRows();
            
            RunOrphanCleanupSproc();

            var rowCounts2 = TableRowCounter.CountRows();
            ObjectAssertions.Equivalent(rowCounts1, rowCounts2);
            var rowCountSum = rowCounts2.Sum(x => x.Records);
       }
    }
}
