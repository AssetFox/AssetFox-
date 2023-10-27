using System;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
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
        public void ToScenarioTreatmentSupersedeRuleEntityOnDomain()
        {
            // Arrange            
            var treatmentSupersedeRule = TreatmentSupersedeRuleTestSetup.TreatmentSupersedeRuleDomain(simulationSource, testSimulation);

            // Act
            var resultEntity = treatmentSupersedeRule.ToScenarioTreatmentSupersedeRuleEntity(TreatmentSupersedeRuleTestSetup.TreatmentEntityId, simulationSource.Id);

            // Assert
            Assert.NotNull(resultEntity);
            Assert.IsType<ScenarioTreatmentSupersedeRuleEntity>(resultEntity);
            Assert.Equal(TestDataForTreatmentSupersedeRules.TreatmentId, resultEntity.TreatmentId);
            Assert.Equal(TestDataForTreatmentSupersedeRules.PreventTreatmentId, resultEntity.PreventScenarioSelectableTreatment.Id);
            Assert.Equal("Prevent Treatment", resultEntity.PreventScenarioSelectableTreatment.Name);
            Assert.NotNull(resultEntity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin);
        }


        [Fact]
        public void ToScenarioTreatmentSupersedeRuleEntityOnDto()
        {
            // Arrange            
            var treatmentSupersedeRuleDto = TreatmentSupersedeRuleTestSetup.TreatmentSuperdedeRuleDto;

            // Act
            var resultEntity = treatmentSupersedeRuleDto.ToScenarioTreatmentSupersedeRuleEntity(treatmentSupersedeRuleDto.treatment.Id, simulationSource.Id);

            // Assert
            Assert.NotNull(resultEntity);
            Assert.IsType<ScenarioTreatmentSupersedeRuleEntity>(resultEntity);
            Assert.Equal(treatmentSupersedeRuleDto.treatment.Id, resultEntity.TreatmentId);
            Assert.Equal("PreventTreatment1", resultEntity.PreventScenarioSelectableTreatment.Name);
            Assert.NotNull(resultEntity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin);
            Assert.Equal("TestExpression", resultEntity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.MergedCriteriaExpression);
        }

        [Fact]
        public void CreateTreatmentSupersedeRule()
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
            Assert.Equal("PreventTreatment1", selectableTreatment.SupersedeRules.FirstOrDefault().Treatment.Name);
        }

        [Fact]
        public void ToDtoOnScenarioTreatmentSupersedeRuleEntity()
        {
            // Arrange
            var treatmentEntity = simulationSource.SelectableTreatments.FirstOrDefault(_ => _.Name == "Prevent Treatment");
            var scenarioTreatmentSupersedeRuleEntity = TestEntitiesForTreatmentSupersedeRules.ScenarioTreatmentSupersedeRule(treatmentDtoWithEmptyLists.Id, treatmentEntity);

            // Act
            var resultDto = scenarioTreatmentSupersedeRuleEntity.ToDto();

            // Assert
            Assert.NotNull(resultDto);
            Assert.IsType<TreatmentSupersedeRuleDTO>(resultDto);
            Assert.Equal(treatmentEntity.Id, resultDto.treatment.Id);
            Assert.Equal(scenarioTreatmentSupersedeRuleEntity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.Name, resultDto.CriterionLibrary.Name);
            Assert.Equal(scenarioTreatmentSupersedeRuleEntity.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.MergedCriteriaExpression, resultDto.CriterionLibrary.MergedCriteriaExpression);
            Assert.Equal(scenarioTreatmentSupersedeRuleEntity.PreventScenarioSelectableTreatment.Name, resultDto.treatment.Name);
        }

        [Fact]
        public void ToDtoOnTreatmentSupersedeRuleEntity()
        {
            var treatmentEntity = TestEntitiesForTreatmentSupersedeRules.SelectablePreventTreatment();
            var treatmentSupersedeRuleEntity = TestEntitiesForTreatmentSupersedeRules.TreatmentSupersedeRule(treatmentDtoWithEmptyLists.Id, treatmentEntity);

            // Act
            var resultDto = treatmentSupersedeRuleEntity.ToDto();

            // Assert
            Assert.NotNull(resultDto);
            Assert.IsType<TreatmentSupersedeRuleDTO>(resultDto);
            Assert.Equal(treatmentEntity.Id, resultDto.treatment.Id);
            Assert.Equal(treatmentSupersedeRuleEntity.CriterionLibraryTreatmentSupersedeRuleJoin.CriterionLibrary.Name, resultDto.CriterionLibrary.Name);
            Assert.Equal(treatmentSupersedeRuleEntity.CriterionLibraryTreatmentSupersedeRuleJoin.CriterionLibrary.MergedCriteriaExpression, resultDto.CriterionLibrary.MergedCriteriaExpression);
            Assert.Equal(treatmentSupersedeRuleEntity.PreventSelectableTreatment.Name, resultDto.treatment.Name);
        }
    }
}
