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

            var mockSimulationRepo = new Mock<ISimulationRepository>();
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Simulation, TestDataForCommittedProjects.Simulations.AsQueryable());

            var mockMaintainableAssetRepo = new Mock<IMaintainableAssetRepository>();
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.MaintainableAsset, TestDataForCommittedProjects.MaintainableAssetEntities.AsQueryable());

            var mockCommittedProjectsRepo = new Mock<ICommittedProjectRepository>();
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.CommittedProject, TestDataForCommittedProjects.CommittedProjectEntities.AsQueryable());

            var mockAttributeRepo = new Mock<IAttributeRepository>();
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, TestDataForCommittedProjects.AttribureEntities.AsQueryable());

            var mockInvestmentPlan = new Mock<IInvestmentPlanRepository>();
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.InvestmentPlan, TestDataForCommittedProjects.InvestmentPlanEntities().AsQueryable());

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

        [Fact(Skip = "Not Implemented")]
        public void GetForSimulationWorksWithoutCommittedProjects()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void GetForSimulationHandlesBadScenarioId()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void GetForExportWorksWithCommittedProjects()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void GetForExportWorksWithoutCommittedProjects()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void GetForExportHandlesBadScenarioId()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void CreateWorksForValidCommittedProjectData()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void CreateHandlesBadSimulationId()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void CreateHandlesNonExistingBudgets()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void CreateWorksWithNullBudgets()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void DeleteWorksWithValidSimulation()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void DeleteHandlesInvalidSimulation()
        {

        }

        [Fact(Skip = "Not Implemented")]
        public void DeleteHandlesSimulationWithNoCommitts()
        {

        }

        #region Helpers
        private Simulation CreateSimulation(Guid simulationId)
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
            _testUOW.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            return simulation;
        }

        #endregion
    }
}
