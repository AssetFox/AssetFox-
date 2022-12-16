using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.Analysis;

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

            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Simulation, TestDataForCommittedProjects.Simulations.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.MaintainableAsset, TestDataForCommittedProjects.MaintainableAssetEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.CommittedProject, TestDataForCommittedProjects.CommittedProjectEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, TestDataForCommittedProjects.AttribureEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.InvestmentPlan, TestDataForCommittedProjects.InvestmentPlanEntities().AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.ScenarioBudget, TestDataForCommittedProjects.ScenarioBudgetEntities.AsQueryable());

            _testUOW = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _mockedContext.Object);
        }

        [Fact]
        public void GetForSimulationWorksWithCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var simulationDomain = CreateSimulation(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id);

            // Act
            repo.GetSimulationCommittedProjects(simulationDomain);

            // Assert
            Assert.Equal(2, simulationDomain.CommittedProjects.Count);
            Assert.Equal(210000, simulationDomain.CommittedProjects.Sum(_ => _.Cost));
        }


        [Fact]
        public void NoTreatmentBeforeCommittedProjects_GetSimulationCommittedProjects_Expected()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var simulationDomain = CreateSimulation(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "FourYearTest").Id);
            var simulationEntity = _testUOW.Context.Simulation.Single(s => s.Id == simulationDomain.Id);
            simulationEntity.NoTreatmentBeforeCommittedProjects = true;
            _testUOW.Context.Simulation.Update(simulationEntity);
            _testUOW.Context.SaveChanges();

            // Act
            repo.GetSimulationCommittedProjects(simulationDomain);

            // Assert
            Assert.Equal(0, simulationDomain.CommittedProjects.Count);
            Assert.Equal(0, simulationDomain.CommittedProjects.Sum(_ => _.Cost));
        }

        [Fact]
        public void GetForSimulationWorksWithoutCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var simulationDomain = CreateSimulation(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "No Commit").Id);

            // Act
            repo.GetSimulationCommittedProjects(simulationDomain);

            // Assert
            Assert.Equal(0, simulationDomain.CommittedProjects.Count);
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
            var result = repo.GetCommittedProjectsForExport(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(210000, result.Sum(_ => _.Cost));
            Assert.True(result.First() is SectionCommittedProjectDTO);
            Assert.Equal(2, result.First().Consequences.Count);
            Assert.Equal(2, result.First().LocationKeys.Count);
        }

        [Fact]
        public void GetForExportWorksWithoutCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetCommittedProjectsForExport(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "No Commit").Id);

            // Assert
            Assert.Equal(0, result.Count);
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
            newProjects.ForEach(_ => _.SimulationId = TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "No Commit").Id);

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
            repo.DeleteSimulationCommittedProjects(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id);
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
            repo.DeleteSimulationCommittedProjects(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "No Commit").Id);

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
            var result = repo.GetSectionCommittedProjectDTOs(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(210000, result.Sum(_ => _.Cost));
            Assert.Equal(2, result.First().Consequences.Count);
            Assert.Equal(2, result.First().LocationKeys.Count);
        }

        [Fact]
        public void GetSectionCommittedProjectsHandlesSimulationWithoutProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetSectionCommittedProjectDTOs(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "No Commit").Id);

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
            var result = repo.GetSimulationId(Guid.Parse("2e9e66df-4436-49b1-ae68-9f5c10656b1b"));

            // Assert
            Assert.Equal(TestDataForCommittedProjects.Simulations.First(_ => _.Name == "Test").Id, result);
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
            var exp = _testUOW.AttributeRepo.GetExplorer();
            var testNetwork = exp.AddNetwork();
            testNetwork.Id = TestDataForCommittedProjects.NetworkId;
            SectionMapper mapper = new(testNetwork);
            foreach (var asset in TestDataForCommittedProjects.MaintainableAssetEntities)
            {
                mapper.CreateMaintainableAsset(asset);
            }
            var simulation = testNetwork.AddSimulation();
            simulation.Id = simulationId;
            // This has to be ignored to create a bad scenario object
            if (populateInvestments) _testUOW.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            return simulation;
        }

        #endregion
    }
}
