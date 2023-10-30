using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using MaintainableAsset = AppliedResearchAssociates.iAM.Data.Networking.MaintainableAsset;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CommittedProjects
{
    public class CommittedProjectRepoTests
    {
        private UnitOfDataPersistenceWork _testUOW;
        private Mock<IAMContext> _mockedContext;
        private Guid _badScenario = Guid.Parse("0c66674c-8fcb-462b-8765-69d6815e0958");

        public CommittedProjectRepoTests()
        {
            var mockedTestUOW = new Mock<IUnitOfWork>();
            _mockedContext = new Mock<IAMContext>();

            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Simulation, TestEntitiesForCommittedProjects.Simulations.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.MaintainableAsset, TestEntitiesForCommittedProjects.MaintainableAssetEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.CommittedProject, TestEntitiesForCommittedProjects.CommittedProjectEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, TestEntitiesForCommittedProjects.AttribureEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.InvestmentPlan, TestEntitiesForCommittedProjects.InvestmentPlanEntities().AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.ScenarioBudget, TestEntitiesForCommittedProjects.ScenarioBudgetEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.ScenarioSelectableTreatment, TestEntitiesForCommittedProjects.FourYearScenarioNoTreatmentEntities().AsQueryable());

            _testUOW = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _mockedContext.Object);
        }

        [Fact]
        public void GetForSimulationWorksWithCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(TestHelper.UnitOfWork);

            // Set up a network with maintainable assets
            Guid networkId = Guid.Parse("502C1684-C8B6-48FD-9725-A2295AA3E0F0");
            var maintainableAssets = new List<MaintainableAsset>();
            var assetId = TestDataForCommittedProjects.MaintainableAssetId1;
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var maintainableAssetEntity = maintainableAsset.ToEntity(networkId);
            var maintainableAssetLocation = new MaintainableAssetLocationEntity()
            {
                Id = Guid.NewGuid(), 
                LocationIdentifier = "3",
                Discriminator = DataPersistenceConstants.SectionLocation,
            };
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            maintainableAssetEntity.MaintainableAssetLocation = maintainableAssetLocation;
            var testMaintainableAsset = maintainableAssetEntity.ToDomain(locationIdentifier);
            maintainableAssets.Add(testMaintainableAsset);
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, maintainableAssets, networkId, TestAttributeIds.CulvDurationNId);

            // Setup a simulation based on network
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee"), "Test Simulation", new Guid(), networkId);
            simulation.NetworkId = network.Id;

            // Set up a selectable treatment for the test with sample budgets
            var treatmentbudget = TreatmentBudgetDtos.Dto();
            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(treatmentId);
            var costId = Guid.NewGuid();
            var costLibraryId = Guid.NewGuid();
            var insertCostEquationId = Guid.NewGuid();
            var cost = TreatmentCostDtos.WithEquationAndCriterionLibrary(costId, insertCostEquationId, costLibraryId, "equation", "mergedCriteriaExpression");
            treatment.Costs.Add(cost);
            treatment.Budgets = new List<TreatmentBudgetDTO>() { treatmentbudget };
            treatment.BudgetIds = new List<Guid> {  };
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulation.Id);

            // Set up committed projects for the test
            List<SectionCommittedProjectDTO> sectionCommittedProjects = CreateTestCommittedProjects(simulation.Id);
            TestHelper.UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(sectionCommittedProjects);

            // Act
            var testSimulation = CreateSimulation(simulation.Id, false);
            testSimulation.Network.Id = Guid.Parse("502C1684-C8B6-48FD-9725-A2295AA3E0F0");
            TestHelper.UnitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(testSimulation);

            // Assert
            Assert.Equal(2, testSimulation.CommittedProjects.Count);
            Assert.Equal(220000, testSimulation.CommittedProjects.Sum(_ => _.Cost));
        }


        [Fact(Skip = "Test is not dependent on the No Treatment Before Committed Project Flag")]
        public void NoTreatmentBeforeCommittedProjects_GetSimulationCommittedProjects_Expected()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var inputSimulationEntity = TestEntitiesForCommittedProjects.Simulations.Single(_ => _.Name == "FourYearTest");
            var simulationDomain = CreateSimulation(inputSimulationEntity.Id);
            var simulationEntity = _testUOW.Context.Simulation.Single(s => s.Id == simulationDomain.Id);
            simulationEntity.NoTreatmentBeforeCommittedProjects = true;
            _testUOW.Context.Simulation.Update(simulationEntity);
            _testUOW.Context.SaveChanges();

            // Act
            repo.GetSimulationCommittedProjects(simulationDomain);

            // Assert
            var committedProjectNames = simulationDomain.CommittedProjects.Select(cp => cp.Name).ToList();
            Assert.Equal(4, simulationDomain.CommittedProjects.Count);
            Assert.Equal(10000, simulationDomain.CommittedProjects.Sum(_ => _.Cost));
            Assert.Equal(3, simulationDomain.CommittedProjects.Count(_ => _.Name != "Something"));
        }

        [Fact]
        public void GetForSimulationWorksWithoutCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(TestHelper.UnitOfWork);

            // Set up a network with maintainable assets
            Guid networkId = Guid.Parse("119AD446-3330-426B-864D-E9D471949D6B");
            var maintainableAssets = new List<MaintainableAsset>();
            var assetId = TestDataForCommittedProjects.MaintainableAssetId2;
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var maintainableAssetEntity = maintainableAsset.ToEntity(networkId);
            var maintainableAssetLocation = new MaintainableAssetLocationEntity()
            {
                Id = Guid.Parse("ffff6f5d-0559-4363-aad0-e13849b8e369"),
                LocationIdentifier = "3",
                Discriminator = DataPersistenceConstants.SectionLocation,
            };
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            maintainableAssetEntity.MaintainableAssetLocation = maintainableAssetLocation;
            var testMaintainableAsset = maintainableAssetEntity.ToDomain(locationIdentifier);
            maintainableAssets.Add(testMaintainableAsset);
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, maintainableAssets, networkId, TestAttributeIds.CulvDurationNId);

            // Setup a simulation based on network
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, TestDataForCommittedProjects.NoCommitSimulationId, "Test Simulation", new Guid(), networkId);
            simulation.NetworkId = network.Id;

            // Set up a selectable treatment for the test with sample budgets
            var testBudget = new TreatmentBudgetDTO
            {
                Id = Guid.NewGuid(),
                Name = "Budget Test 1"
            };
            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(treatmentId);
            var costId = Guid.NewGuid();
            var costLibraryId = Guid.NewGuid();
            var insertCostEquationId = Guid.NewGuid();
            var cost = TreatmentCostDtos.WithEquationAndCriterionLibrary(costId, insertCostEquationId, costLibraryId, "equation", "mergedCriteriaExpression");
            treatment.Costs.Add(cost);
            treatment.Budgets = new List<TreatmentBudgetDTO>() { testBudget };
            treatment.BudgetIds = new List<Guid> { libraryId, treatmentId };
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulation.Id);

            // Act
            var testSimulation = CreateSimulation(simulation.Id, false);
            testSimulation.Network.Id = Guid.Parse("502C1684-C8B6-48FD-9725-A2295AA3E0F0");
            TestHelper.UnitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(testSimulation);

            // Assert
            Assert.Equal(0, testSimulation.CommittedProjects.Count);
        }

        [Fact]
        public void GetForSimulationHandlesBadScenarioId()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var simulationDomain = CreateSimulation(_badScenario, false);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.GetSimulationCommittedProjects(simulationDomain));
        }

        [Fact]
        public void GetForExportWorksWithCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetCommittedProjectsForExport(TestDataForCommittedProjects.SimulationId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(210000, result.Sum(_ => _.Cost));
            Assert.True(result.First() is SectionCommittedProjectDTO);
            Assert.Equal(2, result.First().LocationKeys.Count);
        }

        [Fact]
        public void GetForExportWorksWithoutCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetCommittedProjectsForExport(TestDataForCommittedProjects.NoCommitSimulationId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetForExportHandlesBadScenarioId()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.GetCommittedProjectsForExport(_badScenario));
        }

        [Fact(Skip = "Unable to run with BulkExtensions")]
        public void UpsertWorksForValidCommittedProjectData()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.SimulationId = TestDataForCommittedProjects.NoCommitSimulationId);

            // Act
            repo.UpsertCommittedProjects(newProjects);
        }

        [Fact]
        public void UpsertHandlesBadSimulationId()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.SimulationId = _badScenario);

            // Act & Assert
            var exception = Assert.Throws<RowNotInTableException>(() => repo.UpsertCommittedProjects(newProjects));
            Assert.Contains("simulation ID", exception.Message);
        }

        [Fact]
        public void UpsertHandlesNonExistingBudgets()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.ScenarioBudgetId = Guid.Parse("0d91f67d-d5f4-4c1b-861c-3a5a24aab100"));

            // Act & Assert
            var exception = Assert.Throws<RowNotInTableException>(() => repo.UpsertCommittedProjects(newProjects));
            Assert.Contains("budget IDs", exception.Message);
        }

        [Fact(Skip = "Unable to run with BulkExtensions")]
        public void UpsertWorksWithNullBudgets()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.ScenarioBudgetId = null);

            // Act
            repo.UpsertCommittedProjects(newProjects);
        }

        [Fact(Skip = "Unable to run with BulkExtensions")]
        public void DeleteSimulationWorksWithValidSimulation()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            repo.DeleteSimulationCommittedProjects(TestDataForCommittedProjects.SimulationId);
        }

        [Fact]
        public void DeleteSimulationHandlesInvalidSimulation()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.DeleteSimulationCommittedProjects(_badScenario));
        }

        [Fact]
        public void DeleteSimulationHandlesSimulationWithNoCommitts()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            repo.DeleteSimulationCommittedProjects(TestDataForCommittedProjects.NoCommitSimulationId);

            // No assert required as long as it works
        }

        [Fact(Skip = "Unable to run with BulkExtensions")]
        public void DeleteSpecificWorksWithValidProject()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var projectsToDelete = TestDataForCommittedProjects.ValidCommittedProjects.Select(_ => _.Id).ToList();

            // Act
            repo.DeleteSpecificCommittedProjects(projectsToDelete);
        }

        [Fact]
        public void DeleteSpecificHandlesInvalidProject()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var projectsToDelete = new List<Guid>() { Guid.Parse("ba5645ae-4f13-4a9f-94fd-2c03d26de500") };

            // Act & Assert
            // No assert here.  If a specific project does not exist but others do, we do not want to throw an error
        }

        [Fact]
        public void CanGetSectionCommittedProjectsForSimulationWithProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetSectionCommittedProjectDTOs(TestDataForCommittedProjects.SimulationId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(210000, result.Sum(_ => _.Cost));
            Assert.Equal(2, result.First().LocationKeys.Count);
        }

        [Fact]
        public void GetSectionCommittedProjectsHandlesSimulationWithoutProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetSectionCommittedProjectDTOs(TestDataForCommittedProjects.NoCommitSimulationId);

            // Assert
            Assert.Equal(0, result.Count);
            Assert.IsType<List<SectionCommittedProjectDTO>>(result);
        }

        [Fact]
        public void GetSectionCommittedProjectsHandlesBadSimulations()
        { 
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.GetSectionCommittedProjectDTOs(_badScenario));
        }

        [Fact]
        public void GetSimulationIdReturnsValidValue()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetSimulationId(TestDataForCommittedProjects.CommittedProjectId1);

            // Assert
            Assert.Equal(TestDataForCommittedProjects.SimulationId, result);
        }

        [Fact]
        public void GetSimulationIdHandlesBadProject()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.GetSimulationId(Guid.Parse("aa84643a-24c0-4722-820c-6a1fed01ccac")));
        }

        #region Helpers
        private Simulation CreateSimulation(Guid simulationId, bool populateInvestments = true)
        {
            var explorer = _testUOW.AttributeRepo.GetExplorer();
            var testNetwork = explorer.AddNetwork();
            testNetwork.Id = TestDataForCommittedProjects.NetworkId;
            SectionMapper mapper = new(testNetwork);
            foreach (var asset in TestEntitiesForCommittedProjects.MaintainableAssetEntities)
            {
                mapper.CreateMaintainableAsset(asset);
            }
            var simulation = testNetwork.AddSimulation();
            simulation.Id = simulationId;
            // This has to be ignored to create a bad scenario object
            if (populateInvestments) _testUOW.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            return simulation;
        }

        private List<SectionCommittedProjectDTO> CreateTestCommittedProjects(Guid simulationId)
        {
            List<SectionCommittedProjectDTO> testCommittedProjects = new List<SectionCommittedProjectDTO>();
            var committedProject = SectionCommittedProjectDtos.Dto1(TestDataForCommittedProjects.CommittedProjectId2, simulationId);
            var committedProject2 = SectionCommittedProjectDtos.Dto2(Guid.Parse("c6fd501b-83f3-49e0-a728-8444c14b6262"), simulationId);
            testCommittedProjects.Add(committedProject);
            testCommittedProjects.Add(committedProject2);
            return testCommittedProjects;
        }
        #endregion
    }
}
