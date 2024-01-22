using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TreatmentSupersedeRule
{
    public class TreatmentSupersedeRuleRepositoryTests
    {
        private ScenarioSelectableTreatmentEntity _testScenarioTreatment;
        private ScenarioTreatmentSupersedeRuleEntity _testScenarioTreatmentSupersedeRule;
        private CriterionLibraryScenarioTreatmentSupersedeRuleEntity CriterionLibraryScenarioTreatmentSupersedeRuleJoin;
        private CriterionLibraryEntity criterionLibrary;
        private TreatmentLibraryEntity _testTreatmentLibrary;
        private SelectableTreatmentEntity _testTreatment;
        private TreatmentSupersedeRuleEntity _testTreatmentSupersedeRule;
        private CriterionLibraryTreatmentSupersedeRuleEntity CriterionLibraryTreatmentSupersedeRuleJoin;

        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        [Fact]
        public void UpsertOrDeleteTreatmentSupersedeRules_DoesNotThrow()
        {
            // Arrange
            var dto = new TreatmentLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Treatments = new List<TreatmentDTO> { new TreatmentDTO { Id = Guid.NewGuid(), SupersedeRules = new List<TreatmentSupersedeRuleDTO> { } } }
            };
            var treatmentDto = dto.Treatments.First();
            var dict = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>>
            {
                { treatmentDto.Id, treatmentDto.SupersedeRules }
            };

            // Act            
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteTreatmentSupersedeRules(dict, dto.Id);
        }

        [Fact]
        public void UpsertOrDeleteTreatmentSupersedeRules_ValidInput_SucceedsUpdate()
        {
            // Arrange                      
            CreateLibraryTestData();

            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(_testTreatmentLibrary.Id);
            dtos[0].SupersedeRules = new List<TreatmentSupersedeRuleDTO>() { _testTreatmentSupersedeRule.ToDto(dtos) };
            dtos[0].SupersedeRules[0].CriterionLibrary.MergedCriteriaExpression = criterionLibrary.MergedCriteriaExpression + " and AGE<10";
            var supersedeRulesPerTreatmentId = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>>
            {
                { _testTreatment.Id, dtos[0].SupersedeRules }
            };

            // Act
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteTreatmentSupersedeRules(supersedeRulesPerTreatmentId, _testTreatmentLibrary.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(_testTreatmentLibrary.Id);

            Assert.NotEqual(dtos[0].SupersedeRules[0].CriterionLibrary.Id, modifiedDto[0].SupersedeRules[0].CriterionLibrary.Id);
            Assert.Equal(dtos[0].SupersedeRules[0].CriterionLibrary.MergedCriteriaExpression, modifiedDto[0].SupersedeRules[0].CriterionLibrary.MergedCriteriaExpression);
        }

        [Fact]
        public void UpsertOrDeleteTreatmentSupersedeRules_ValidInput_SucceedsAdd()
        {
            // Arrange                      
            CreateLibraryTestData();

            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(_testTreatmentLibrary.Id);
            // 2nd rule
            var supersedeRuleDto = _testTreatmentSupersedeRule.ToDto(dtos); ;
            var supersedeRuleDto2 = new TreatmentSupersedeRuleDTO { CriterionLibrary = new CriterionLibraryDTO(), Id = Guid.NewGuid(), treatment = new TreatmentDTO() };
            dtos[0].SupersedeRules = new List<TreatmentSupersedeRuleDTO>() { supersedeRuleDto, supersedeRuleDto2 };
            dtos[0].SupersedeRules[0].Id = Guid.NewGuid();
            var supersedeRulesPerTreatmentId = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>>
            {
                { _testTreatment.Id, dtos[0].SupersedeRules }
            };
            var treatmentSupersedeRulesCntBefore = GetSupersedeRulesCount(dtos[0]);

            // Act
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteTreatmentSupersedeRules(supersedeRulesPerTreatmentId, _testTreatmentLibrary.Id);

            // Assert
            var treatmentSupersedeRulesCntAfter = GetSupersedeRulesCount(dtos[0]);
            Assert.Equal(treatmentSupersedeRulesCntBefore + 1, treatmentSupersedeRulesCntAfter);
        }

        [Fact]
        public void UpsertOrDeleteScenarioTreatmentSupersedeRules_DoesNotThrow()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var treatmentDto = new TreatmentDTO { Id = Guid.NewGuid(), SupersedeRules = new List<TreatmentSupersedeRuleDTO> { } };
            var dict = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>>
            {
                { treatmentDto.Id, treatmentDto.SupersedeRules }
            };

            // Act
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteScenarioTreatmentSupersedeRules(dict, simulation.Id);
        }

        [Fact]
        public void UpsertOrDeleteScenarioTreatmentSupersedeRules_ValidInput_Succeeds()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);

            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation.Id);            
            dtos[0].SupersedeRules = new List<TreatmentSupersedeRuleDTO>() { _testScenarioTreatmentSupersedeRule.ToDto(dtos) };
            dtos[0].SupersedeRules[0].CriterionLibrary.MergedCriteriaExpression = criterionLibrary.MergedCriteriaExpression + " and AGE<10";
            var supersedeRulesPerTreatmentId = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>>
            {
                { _testScenarioTreatment.Id, dtos[0].SupersedeRules }
            };

            // Act
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteScenarioTreatmentSupersedeRules(supersedeRulesPerTreatmentId, simulation.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation.Id);

            Assert.NotEqual(dtos[0].SupersedeRules[0].CriterionLibrary.Id, modifiedDto[0].SupersedeRules[0].CriterionLibrary.Id);
            Assert.Equal(dtos[0].SupersedeRules[0].CriterionLibrary.MergedCriteriaExpression, modifiedDto[0].SupersedeRules[0].CriterionLibrary.MergedCriteriaExpression);
        }

        [Fact]
        public void GetScenarioTreatmentSupersedeRulesWithTwoParams_Succeeds()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);

            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation.Id);           

            // Act
            var result = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetScenarioTreatmentSupersedeRules(_testScenarioTreatment.Id, simulation.Id);

            // Assert
            Assert.IsType<List<TreatmentSupersedeRuleDTO>>(result);
            Assert.Equal(1, result?.Count);
        }

        [Fact]
        public void GetScenarioTreatmentSupersedeRulesBysimulationId_Succeeds()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);

            // Act
            var result = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetScenarioTreatmentSupersedeRulesBysimulationId(simulation.Id);

            // Assert
            Assert.IsType<List<TreatmentSupersedeRuleExportDTO>>(result);
            Assert.Equal(1, result?.Count);
        }

        [Fact]
        public void GetLibraryTreatmentSupersedeRulesWithTwoParams_Succeeds()
        {
            // Arrange                        
            CreateLibraryTestData();
            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(_testTreatmentLibrary.Id);

            // Act
            var result = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetLibraryTreatmentSupersedeRules(_testTreatment.Id, _testTreatmentLibrary.Id);

            // Assert
            Assert.IsType<List<TreatmentSupersedeRuleDTO>>(result);
            Assert.Equal(1, result?.Count);
        }

        [Fact]
        public void GetLibraryTreatmentSupersedeRulesBysimulationId_Succeeds()
        {
            // Arrange
            CreateLibraryTestData();
            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(_testTreatmentLibrary.Id);

            // Act
            var result = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetLibraryTreatmentSupersedeRulesByLibraryId(_testTreatmentLibrary.Id);

            // Assert
            Assert.IsType<List<TreatmentSupersedeRuleExportDTO>>(result);
            Assert.Equal(1, result?.Count);
        }

        private void CreateScenarioTestData(Guid simulationId)
        {
            _testScenarioTreatment = new ScenarioSelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Scenario Treatment",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatment);

            var supersedeRuleId = Guid.NewGuid();
            _testScenarioTreatmentSupersedeRule = new ScenarioTreatmentSupersedeRuleEntity
            {
                Id = supersedeRuleId,
                TreatmentId = _testScenarioTreatment.Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentSupersedeRule);

            var criterionLibraryId = Guid.NewGuid();
            criterionLibrary = new CriterionLibraryEntity { Id = criterionLibraryId, Name = "Test Library", IsSingleUse = true, MergedCriteriaExpression = "Age>5" };
            TestHelper.UnitOfWork.Context.AddEntity(criterionLibrary);

            CriterionLibraryScenarioTreatmentSupersedeRuleJoin = new CriterionLibraryScenarioTreatmentSupersedeRuleEntity { CriterionLibraryId = criterionLibraryId, ScenarioTreatmentSupersedeRuleId = supersedeRuleId };
            TestHelper.UnitOfWork.Context.AddEntity(CriterionLibraryScenarioTreatmentSupersedeRuleJoin);

            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private void CreateLibraryTestData()
        {
            _testTreatmentLibrary = new TreatmentLibraryEntity { Id = Guid.NewGuid(), Name = "Test Treatment Library" };
            TestHelper.UnitOfWork.Context.TreatmentLibrary.Add(_testTreatmentLibrary);

            _testTreatment = new SelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                TreatmentLibraryId = _testTreatmentLibrary.Id,
                Name = "Test Treatment",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatment);

            _testTreatmentSupersedeRule = SetupSupersedeRule();

            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private TreatmentSupersedeRuleEntity SetupSupersedeRule()
        {
            var supersedeRuleId = Guid.NewGuid();
            var testTreatmentSupersedeRule = new TreatmentSupersedeRuleEntity
            {
                Id = supersedeRuleId,
                TreatmentId = _testTreatment.Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(testTreatmentSupersedeRule);

            var criterionLibraryId = Guid.NewGuid();
            criterionLibrary = new CriterionLibraryEntity { Id = criterionLibraryId, Name = "Test Criterion Library", IsSingleUse = true, MergedCriteriaExpression = "Age>1" };
            TestHelper.UnitOfWork.Context.AddEntity(criterionLibrary);

            CriterionLibraryTreatmentSupersedeRuleJoin = new CriterionLibraryTreatmentSupersedeRuleEntity { CriterionLibraryId = criterionLibraryId, TreatmentSupersedeRuleId = supersedeRuleId };
            TestHelper.UnitOfWork.Context.AddEntity(CriterionLibraryTreatmentSupersedeRuleJoin);
            return testTreatmentSupersedeRule;
        }

        private int GetSupersedeRulesCount(TreatmentDTO treatmentDTO) =>
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetLibraryTreatmentSupersedeRules(treatmentDTO.Id, _testTreatmentLibrary.Id).Count;        
    }
}
