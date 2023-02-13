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
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CommittedProjects
{
    public class CommittedProjectMapperTests
    {
        private Simulation testSimulation;
        private SimulationEntity simulationSource;

        public CommittedProjectMapperTests()
        {
            simulationSource = TestEntitiesForCommittedProjects.Simulations.Single(_ => _.Name == "FourYearTest");
            var debugExplorer = new Explorer("dummy");
            var testNetwork = simulationSource.Network.ToDomain(debugExplorer);
            simulationSource.CreateSimulation(testNetwork, DateTime.Now, DateTime.Now);
            testSimulation = testNetwork.Simulations.First();
            simulationSource.InvestmentPlan.FillSimulationInvestmentPlan(testSimulation);
        }

        [Fact]
        public void MapperAddsNoTreatmentWhenFlagEnabled()
        {
            // Arrange
            var providedCommittedProject = simulationSource.CommittedProjects.First(_ => _.Year == 2025);
            var testAssetId = simulationSource.Network.MaintainableAssets.First().Id;
            var noTreatment = TestEntitiesForCommittedProjects.FourYearScenarioNoTreatment();

            // Act
            providedCommittedProject.CreateCommittedProject(
                testSimulation,
                testAssetId,
                true,
                0,
                noTreatment);

            // Assert
            Assert.Equal(4, testSimulation.CommittedProjects.Count);
            Assert.Single(testSimulation.CommittedProjects.Where(_ => _.Name == providedCommittedProject.Name));
            Assert.Equal(3, testSimulation.CommittedProjects.Where(_ => _.Name == noTreatment.Name).Count());
        }

        [Fact]
        public void MapperDoesNotAddNoTreatmentWhenFlagDisabled()
        {
            // Arrange
            var providedCommittedProject = simulationSource.CommittedProjects.First();
            var testAssetId = simulationSource.Network.MaintainableAssets.First().Id;

            // Act
            providedCommittedProject.CreateCommittedProject(
                testSimulation,
                testAssetId,
                false,
                0,
                TestEntitiesForCommittedProjects.FourYearScenarioNoTreatment());

            // Assert
            Assert.Equal(1, testSimulation.CommittedProjects.Count);
        }

        [Fact]
        public void MapperHandlesAssetWithMultipleProjectsInDifferentYears()
        {
            // Arrange
            var firstCommittedProject = simulationSource.CommittedProjects.First();
            var secondCommittedProject = simulationSource.CommittedProjects.Last();
            secondCommittedProject.Name = "Something Else";
            var testAssetId = simulationSource.Network.MaintainableAssets.First().Id;
            var noTreatment = TestEntitiesForCommittedProjects.FourYearScenarioNoTreatment();

            // Act
            firstCommittedProject.CreateCommittedProject(testSimulation, testAssetId, true, 0, noTreatment);
            secondCommittedProject.CreateCommittedProject(testSimulation, testAssetId, true, 0, noTreatment);

            // Assert
            Assert.Equal(4, testSimulation.CommittedProjects.Count);
            Assert.Single(testSimulation.CommittedProjects.Where(_ => _.Name == firstCommittedProject.Name));
            Assert.Single(testSimulation.CommittedProjects.Where(_ => _.Name == secondCommittedProject.Name));
            Assert.Equal(2, testSimulation.CommittedProjects.Where(_ => _.Name == noTreatment.Name).Count());
        }

        [Fact(Skip = "This should not be handled by the mapper")]
        public void MapperHandlesAssetWithMultipleProjectsInSameYear()
        {
            // Arrange
            var firstCommittedProject = simulationSource.CommittedProjects.First(_ => _.Year == 2025);
            var secondCommittedProject = simulationSource.CommittedProjects.First(_ => _.Year != 2025);
            secondCommittedProject.Name = "Something Else";
            secondCommittedProject.Year = firstCommittedProject.Year;
            var testAssetId = simulationSource.Network.MaintainableAssets.First().Id;
            var noTreatment = TestEntitiesForCommittedProjects.FourYearScenarioNoTreatment();

            // Act
            firstCommittedProject.CreateCommittedProject(testSimulation, testAssetId, true, 0, noTreatment);
            secondCommittedProject.CreateCommittedProject(testSimulation, testAssetId, true, 0, noTreatment);

            // Assert
            Assert.Equal(4, testSimulation.CommittedProjects.Count);
            Assert.Single(testSimulation.CommittedProjects.Where(_ => _.Name == secondCommittedProject.Name));
            Assert.Equal(3, testSimulation.CommittedProjects.Where(_ => _.Name == noTreatment.Name).Count());
        }
    }
}
