using System;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TreatmentSupersedeRule
{
    public class TreatmentSupersedeRuleMapperTests
    {
        private TreatmentDTO treatmentDtoWithEmptyLists;        
        private SimulationEntity simulationSource;
        private Simulation testSimulation;

        public TreatmentSupersedeRuleMapperTests()
        {
            simulationSource = TestEntitiesForTreatmentSupersedeRules.GoodTestSimulation();
            var debugExplorer = new Explorer("dummy");
            var testNetwork = simulationSource.Network.ToDomain(debugExplorer);
            simulationSource.CreateSimulation(testNetwork, DateTime.Now, DateTime.Now);
            testSimulation = testNetwork.Simulations.First();
            treatmentDtoWithEmptyLists = TreatmentDtos.DtoWithEmptyListsWithCriterionLibrary(Guid.NewGuid(), "Test Treatmnent1");
        }

        [Fact]
        public void ToScenarioTreatmentSupersedeRuleEntityOnValidDomain()
        {
            // Arrange            
            var treatmentSupersedeRule = TreatmentSupersedeRuleTestSetup.TreatmentSupersedeRuleDomain(simulationSource, testSimulation);

            // Act
            var resultEntity = treatmentSupersedeRule.ToScenarioTreatmentSupersedeRuleEntity(TreatmentSupersedeRuleTestSetup.TreatmentEntityId, simulationSource.Id);

            // Assert
            Assert.NotNull(resultEntity);
            Assert.IsType<ScenarioTreatmentSupersedeRuleEntity>(resultEntity);
            Assert.Equal(TestDataForTreatmentSupersedeRules.TreatmentId, resultEntity.TreatmentId);
            Assert.Equal(TestDataForTreatmentSupersedeRules.PreventTreatmentId, resultEntity.PreventTreatmentId);           
            Assert.NotNull(resultEntity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin);
        }

        [Fact]
        public void ToScenarioTreatmentSupersedeRuleEntityOnValidDto()
        {
            // Arrange            
            var treatmentSupersedeRuleDto = TreatmentSupersedeRuleTestSetup.TreatmentSuperdedeRuleDto;

            // Act
            var resultEntity = treatmentSupersedeRuleDto.ToScenarioTreatmentSupersedeRuleEntity(simulationSource.SelectableTreatments.FirstOrDefault().Id);

            // Assert
            Assert.NotNull(resultEntity);
            Assert.IsType<ScenarioTreatmentSupersedeRuleEntity>(resultEntity);
            Assert.Equal(treatmentSupersedeRuleDto.treatment.Id, resultEntity.PreventTreatmentId);
            Assert.NotNull(resultEntity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin);
            Assert.Equal("TestExpression", resultEntity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.MergedCriteriaExpression);
        }

        [Fact]
        public void CreateTreatmentSupersedeRuleWithCorrectProperties()
        {
            // Arrange
            var treatmentEntity = simulationSource.SelectableTreatments.FirstOrDefault(_ => _.Name == "TestTreatmentWithRules");
            var treatmentSupersedeRuleEntity = treatmentEntity.ScenarioTreatmentSupersedeRules.FirstOrDefault();
            var selectableTreatment = TreatmentSupersedeRuleTestSetup.TreatmentSupersedeDomain(simulationSource, testSimulation);
            var cntBefore = selectableTreatment.SupersedeRules.Count;

            // Act
            treatmentSupersedeRuleEntity.CreateTreatmentSupersedeRule(selectableTreatment, testSimulation);

            // Assert on selectableTreatment
            Assert.True(cntBefore == 0);
            Assert.True(selectableTreatment.SupersedeRules.Any());
            Assert.Equal("Prevent Treatment", selectableTreatment.SupersedeRules.FirstOrDefault().Treatment.Name);
        }

        [Fact]
        public void ToDtoOnScenarioTreatmentSupersedeRuleEntityWithCorrectProperties()
        {
            // Arrange
            var treatmentEntity = simulationSource.SelectableTreatments.FirstOrDefault(_ => _.Name == "Prevent Treatment");
            var scenarioTreatmentSupersedeRuleEntity = TestEntitiesForTreatmentSupersedeRules.ScenarioTreatmentSupersedeRule(treatmentDtoWithEmptyLists.Id, treatmentEntity);
            var treatmentDtos = TreatmentSupersedeRuleTestSetup.TreatmentDtos;

            // Act
            var resultDto = scenarioTreatmentSupersedeRuleEntity.ToDto(treatmentDtos);

            // Assert
            Assert.NotNull(resultDto);
            Assert.IsType<TreatmentSupersedeRuleDTO>(resultDto);
            Assert.Equal(treatmentEntity.Id, resultDto.treatment.Id);
            Assert.Equal(scenarioTreatmentSupersedeRuleEntity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.Name, resultDto.CriterionLibrary.Name);
            Assert.Equal(scenarioTreatmentSupersedeRuleEntity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.MergedCriteriaExpression, resultDto.CriterionLibrary.MergedCriteriaExpression);
            Assert.Equal(scenarioTreatmentSupersedeRuleEntity.PreventTreatmentId, resultDto.treatment.Id);
        }

        [Fact]
        public void ToDtoOnTreatmentSupersedeRuleEntityWithCorrectProperties()
        {
            var treatmentEntity = TestEntitiesForTreatmentSupersedeRules.SelectablePreventTreatment();
            var treatmentSupersedeRuleEntity = TestEntitiesForTreatmentSupersedeRules.TreatmentSupersedeRule(treatmentDtoWithEmptyLists.Id, treatmentEntity);
            var treatmentDtos = TreatmentSupersedeRuleTestSetup.TreatmentDtos;

            // Act // // TODO dto List
            var resultDto = treatmentSupersedeRuleEntity.ToDto(treatmentDtos);

            // Assert
            Assert.NotNull(resultDto);
            Assert.IsType<TreatmentSupersedeRuleDTO>(resultDto);
            Assert.Equal(treatmentEntity.Id, resultDto.treatment.Id);
            Assert.Equal(treatmentSupersedeRuleEntity.CriterionLibraryTreatmentSupersedeRuleJoin.CriterionLibrary.Name, resultDto.CriterionLibrary.Name);
            Assert.Equal(treatmentSupersedeRuleEntity.CriterionLibraryTreatmentSupersedeRuleJoin.CriterionLibrary.MergedCriteriaExpression, resultDto.CriterionLibrary.MergedCriteriaExpression);
            Assert.Equal(treatmentSupersedeRuleEntity.PreventTreatmentId, resultDto.treatment.Id);
        }
    }
}
