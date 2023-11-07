using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment
{
    public class SelectableTreatmentMapperTests
    {
        private TreatmentDTO treatmentDtoWithEmptyLists;
        private TreatmentDTO treatmentDto;
        private SimulationEntity simulationSource;
        private Simulation testSimulation;

        public SelectableTreatmentMapperTests()
        {
            simulationSource = TestEntitiesForSelectableTreatments.GoodTestSimulation();
            var debugExplorer = new Explorer("dummy");
            var testNetwork = simulationSource.Network.ToDomain(debugExplorer);
            simulationSource.CreateSimulation(testNetwork, DateTime.Now, DateTime.Now);
            testSimulation = testNetwork.Simulations.First();            
            treatmentDtoWithEmptyLists = TreatmentDtos.DtoWithEmptyListsWithCriterionLibrary(Guid.NewGuid(), "Test Treatmnent1");
            treatmentDto = SelectableTreatmentTestSetup.CreateTreatmentDtoWithSupersedeRules("Test Treatment2", treatmentDtoWithEmptyLists);
        }

        [Fact]
        public void ToScenarioEntityOnValidTreatmentDTOWthScenarioTreatmentSupersedeRules()
        {
            // Act
            var resultEntity = treatmentDto.ToScenarioEntityWithCriterionLibraryWithChildren(testSimulation.Id, TestEntitiesForSelectableTreatments.AttribureEntities);

            // Assert
            Assert.NotNull(resultEntity);
            Assert.IsType<ScenarioSelectableTreatmentEntity>(resultEntity);
            Assert.Equal("Test Treatment2", resultEntity.Name);
            Assert.True(resultEntity.ScenarioTreatmentSupersedeRules.Count == 1);
            Assert.True(resultEntity.ScenarioTreatmentCosts.Count == 0);
            Assert.True(resultEntity.ScenarioTreatmentConsequences.Count == 0);
            Assert.True(resultEntity.ScenarioSelectableTreatmentScenarioBudgetJoins.Count == 0);
        }

        [Fact]
        public void CreateSelectableTreatmentAnalysisObjectForNoTreatment()
        {
            // Arrange
            var treatmentEntity = simulationSource.SelectableTreatments.First();

            // Act
            var selectableTreatment = treatmentEntity.CreateSelectableTreatment(testSimulation);

            // Assert
            Assert.NotNull(selectableTreatment);
            Assert.IsType<Analysis.SelectableTreatment>(selectableTreatment);
            Assert.Equal("No Treatment", selectableTreatment.Name);
            Assert.Equal(treatmentEntity.Id, selectableTreatment.Id);
            Assert.True(selectableTreatment.Costs.Count == 1);
            Assert.True(selectableTreatment.Budgets.Count == 0);
            Assert.True(testSimulation.Treatments.Count == 1);
            Assert.True(selectableTreatment.SupersedeRules.Count == 0);
        }

        [Fact]
        public void ToDomainOnTestTreatmentWithSupersedeRulesCosts()
        {
            // Arrange
            var treatmentEntity = simulationSource.SelectableTreatments.FirstOrDefault(_ => _.Name == "TestTreatmentWithRules");

            // Act
            var selectableTreatment = treatmentEntity.ToDomain(testSimulation);

            // Assert
            Assert.NotNull(selectableTreatment);
            Assert.IsType<Analysis.SelectableTreatment>(selectableTreatment);
            Assert.Equal("TestTreatmentWithRules", selectableTreatment.Name);
            Assert.Equal(treatmentEntity.Id, selectableTreatment.Id);
            Assert.True(selectableTreatment.Costs.Count == 1);
            Assert.True(selectableTreatment.Budgets.Count == 0);
            Assert.True(selectableTreatment.PerformanceCurveAdjustmentFactors.Count == 0);            
            Assert.True(selectableTreatment.SupersedeRules.Count == 1);            
        }                

        [Fact]
        public void ScenarioSelectableTreatmentEntityToDtoWithCorrectProperties()
        {
            // Arrange
            var treatmentEntity = simulationSource.SelectableTreatments.FirstOrDefault(_ => _.Name == "TestTreatmentWithRules");

            // Act
            var resultDto = treatmentEntity.ToDto(new List<TreatmentDTO> { treatmentDto });

            // Assert
            Assert.NotNull(resultDto);
            Assert.IsType<TreatmentDTO>(resultDto);
            Assert.Equal(Guid.Empty, resultDto.LibraryId);
            Assert.True(resultDto.Costs.Count == 1 && resultDto.Costs.FirstOrDefault().Equation.Expression.Equals("100"));
            Assert.True(resultDto.Budgets.Count == 1);
            Assert.True(resultDto.PerformanceFactors.Count == 0);
            Assert.True(resultDto.Consequences.Count == 0);
            Assert.True(resultDto.SupersedeRules.Count == 1);
        }

        [Fact]
        public void SelectableTreatmentEntityToDtoWithCorrectProperties()
        {
            // Arrange
            var treatmentEntity = TestEntitiesForSelectableTreatments.Treatment("SelectableTreatment", Guid.NewGuid());

            // Act
            var resultDto = treatmentEntity.ToDto(new List<TreatmentDTO> { treatmentDto });

            // Assert
            Assert.NotNull(resultDto);
            Assert.IsType<TreatmentDTO>(resultDto);
            Assert.Equal(Guid.Empty, resultDto.LibraryId);
            Assert.True(resultDto.Costs.Count == 1 && resultDto.Costs.FirstOrDefault().Equation.Expression.Equals("TestEquationExpression"));
            Assert.True(resultDto.PerformanceFactors.Count == 0);
            Assert.True(resultDto.Consequences.Count == 0);
           Assert.True(resultDto.SupersedeRules.Count == 1);
        }
    }
}
