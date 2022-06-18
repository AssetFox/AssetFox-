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
        public void CreateWorksForValidCommittedProjectData()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.SimulationId = TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "No Commit").Id);

            // Act
            repo.CreateCommittedProjects(newProjects);
        }

        [Fact]
        public void CreateHandlesBadSimulationId()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.SimulationId = _badScenario);

            // Act & Assert
            var exception = Assert.Throws<RowNotInTableException>(() => repo.CreateCommittedProjects(newProjects));
            Assert.Contains("simulation ID", exception.Message);
        }

        [Fact]
        public void CreateHandlesNonExistingBudgets()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.ScenarioBudgetId = Guid.Parse("0d91f67d-d5f4-4c1b-861c-3a5a24aab100"));

            // Act & Assert
            var exception = Assert.Throws<RowNotInTableException>(() => repo.CreateCommittedProjects(newProjects));
            Assert.Contains("budget IDs", exception.Message);
        }

        [Fact(Skip = "Unable to run with BulkExtensions")]
        public void CreateWorksWithNullBudgets()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.ScenarioBudgetId = null);

            // Act
            repo.CreateCommittedProjects(newProjects);
        }

        [Fact(Skip = "Unable to run with BulkExtensions")]
        public void DeleteWorksWithValidSimulation()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            repo.DeleteCommittedProjects(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id);
        }

        [Fact]
        public void DeleteHandlesInvalidSimulation()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.DeleteCommittedProjects(_badScenario));
        }

        [Fact]
        public void DeleteHandlesSimulationWithNoCommitts()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            repo.DeleteCommittedProjects(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "No Commit").Id);

            // No assert required as long as it works
        }

        #region Helpers
        private Simulation CreateSimulation(Guid simulationId, bool populateInvestments = true)
        {
            var exp = _testUOW.AttributeRepo.GetExplorer();
            var testNetwork = exp.AddNetwork();
            testNetwork.Id = TestDataForCommittedProjects.NetworkId;
            foreach (var asset in TestDataForCommittedProjects.MaintainableAssetEntities)
            {
                asset.CreateMaintainableAsset(testNetwork);
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
