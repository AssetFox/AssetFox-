﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.TestHelpers;
using System.Runtime.InteropServices;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.SelectableTreatment
{
    public class SelectableTreatmentRepositoryTests
    {
        private TreatmentLibraryEntity _testTreatmentLibrary;
        private SelectableTreatmentEntity _testTreatment;
        private TreatmentCostEntity _testTreatmentCost;
        private ConditionalTreatmentConsequenceEntity _testTreatmentConsequence;
        private ScenarioSelectableTreatmentEntity _testScenarioTreatment;
        private ScenarioTreatmentCostEntity _testScenarioTreatmentCost;
        private ScenarioConditionalTreatmentConsequenceEntity _testScenarioTreatmentConsequence;


        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }


        private void CreateLibraryTestData()
        {
            _testTreatmentLibrary = new TreatmentLibraryEntity { Id = Guid.NewGuid(), Name = "Test Name" };
            TestHelper.UnitOfWork.Context.TreatmentLibrary.Add(_testTreatmentLibrary);

            _testTreatment = new SelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                TreatmentLibraryId = _testTreatmentLibrary.Id,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatment);

            _testTreatmentCost = new TreatmentCostEntity { Id = Guid.NewGuid(), TreatmentId = _testTreatment.Id };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatmentCost);

            _testTreatmentConsequence = new ConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                SelectableTreatmentId = _testTreatment.Id,
                ChangeValue = "1",
                AttributeId = TestHelper.UnitOfWork.Context.Attribute.First().Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatmentConsequence);

            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private ScenarioBudgetEntity CreateScenarioTestData(Guid simulationId)
        {
            var budget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name"
            };
            TestHelper.UnitOfWork.Context.AddEntity(budget);


            _testScenarioTreatment = new ScenarioSelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatment);
            TestHelper.UnitOfWork.Context.AddEntity(new ScenarioSelectableTreatmentScenarioBudgetEntity
            {
                ScenarioBudgetId = budget.Id,
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            });

            _testScenarioTreatmentCost = new ScenarioTreatmentCostEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentCost);


            _testScenarioTreatmentConsequence = new ScenarioConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id,
                ChangeValue = "1",
                AttributeId = TestHelper.UnitOfWork.Context.Attribute.First().Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentConsequence);


            TestHelper.UnitOfWork.Context.SaveChanges();
            return budget;
        }

        [Fact]
        public void ShouldGetSimpleTreatmentsByLibraryId()
        {
            // Arrange
            Setup();
            CreateLibraryTestData();

            // Act
            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSimpleTreatmentsByLibraryId(_testTreatmentLibrary.Id);
            Assert.Single(dtos);

            Assert.Equal(_testTreatment.Id, dtos[0].Id);
            Assert.Equal(_testTreatment.Name, dtos[0].Name);
        }


        [Fact]
        public void ShouldGetSimpleTreatmentsByScenarioId()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var budget = CreateScenarioTestData(simulation.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSimpleTreatmentsBySimulationId(simulation.Id);

            Assert.Single(dtos);

            Assert.Equal(_testScenarioTreatment.Id, dtos[0].Id);

        }



        [Fact]
        public void ShouldReturnOkResultOnLibraryGet()
        {
            Setup();
            // Act
            var result = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibrariesNoChildren();

            Assert.NotEmpty(result);
        }

        [Fact]
        public void ShouldReturnOkResultOnScenarioGet()
        {
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            // Act
            var result = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation.Id);
        }

        [Fact]
        public void GetTreatmentLibraryWithSingleTreatmentByTreatmentId_TreatmentInDb_Expected()
        {
            // Arrange
            Setup();
            CreateLibraryTestData();

            // Act
            var libraryDTO = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetTreatmentLibraryWithSingleTreatmentByTreatmentId(_testTreatment.Id);

            // Assert
            Assert.Equal(_testTreatmentLibrary.Id, libraryDTO.Id);
            var dto = libraryDTO.Treatments[0];

            Assert.Equal(_testTreatment.Id, dto.Id);
            Assert.Single(dto.Consequences);
            Assert.Single(dto.Costs);

            Assert.Equal(_testTreatmentConsequence.Id, dto.Consequences[0].Id);
            Assert.Equal(_testTreatmentCost.Id, dto.Costs[0].Id);
        }

        [Fact]
        public void GetScenarioSelectableTreatmentById_TreatmentInDb_Expected()
        {
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var budget = CreateScenarioTestData(simulation.Id);

            // Act
            var treatmentDtoWithSimulationId = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatmentById(_testScenarioTreatment.Id);

            // Assert
            Assert.Equal(simulation.Id, treatmentDtoWithSimulationId.SimulationId);
            var treatmentDto = treatmentDtoWithSimulationId.Treatment;
            Assert.Equal(_testScenarioTreatment.Id, treatmentDto.Id);
            Assert.Single(treatmentDto.Consequences);
            Assert.Single(treatmentDto.Costs);
            Assert.Single(treatmentDto.BudgetIds);

            Assert.Equal(_testScenarioTreatmentConsequence.Id, treatmentDto.Consequences[0].Id);
            Assert.Equal(_testScenarioTreatmentCost.Id, treatmentDto.Costs[0].Id);
            Assert.Contains(budget.Id, treatmentDto.BudgetIds);
        }

        [Fact]
        public void ShouldReturnOkResultOnLibraryPost()
        {
            // Arrange
            Setup();
            var dto = new TreatmentLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Treatments = new List<TreatmentDTO>()
            };

            // Act
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(dto);
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dto.Treatments, dto.Id);
        }



        [Fact]
        public void ShouldReturnOkResultOnScenarioPost()
        {
            // Arrange
            Setup();
            var dtos = new List<TreatmentDTO>();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            // Act
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulation.Id);
        }

        [Fact]
        public void ShouldReturnOkResultOnLibraryDelete()
        {
            // we pass if this does not throw.
            TestHelper.UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(Guid.NewGuid());
        }


        [Fact]
        public void ShouldGetLibraryTreatmentData()
        {
            //Arrange
            Setup();
            CreateLibraryTestData();

            // Act
            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibrariesNoChildren();

            Assert.Contains(dtos, t => t.Id == _testTreatmentLibrary.Id);
        }

        [Fact]
        public async Task ShouldGetScenarioTreatmentData()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var budget = CreateScenarioTestData(simulation.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation.Id);

            Assert.Single(dtos);

            Assert.Equal(_testScenarioTreatment.Id, dtos[0].Id);
            Assert.Single(dtos[0].Consequences);
            Assert.Single(dtos[0].Costs);
            Assert.Single(dtos[0].BudgetIds);

            Assert.Equal(_testScenarioTreatmentConsequence.Id, dtos[0].Consequences[0].Id);
            Assert.Equal(_testScenarioTreatmentCost.Id, dtos[0].Costs[0].Id);
            Assert.Contains(budget.Id, dtos[0].BudgetIds);
        }

        [Fact]
        public void ShouldModifyLibraryTreatmentData()
        {
            // Arrange
            Setup();
            CreateLibraryTestData();

            var dto = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibrariesNoChildren();
            var dtoLibrary = dto.Where(t => t.Name == "Test Name").FirstOrDefault();
            var treatments = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(dtoLibrary.Id);
            dtoLibrary.Description = "Updated Description";
            treatments[0].Name = "Updated Name";
            treatments[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            treatments[0].Costs[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            treatments[0].Costs[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            treatments[0].Consequences[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            treatments[0].Consequences[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };

            // Act
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(dtoLibrary);
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dtoLibrary.Treatments, dtoLibrary.Id);

            // Assert
            var modifiedDto =
                TestHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries().Single(lib => lib.Id == dtoLibrary.Id);
            Assert.Equal(dtoLibrary.Description, modifiedDto.Description);
        }

        [Fact]
        public void ShouldModifyScenarioTreatmentData()
        {
            // Arrange
            Setup();
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);

            var scenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = "",
                SimulationId = simulation.Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(scenarioBudget);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var dto = TestHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);

            dto[0].Description = "Updated Description";
            dto[0].Name = "Updated Name";
            dto[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dto[0].Costs[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dto[0].Costs[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            dto[0].Consequences[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dto[0].Consequences[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            dto[0].BudgetIds.Add(scenarioBudget.Id);

            // Act
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dto, simulation.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);
            Assert.Equal(dto[0].Description, modifiedDto[0].Description);
            Assert.Equal(dto[0].Name, modifiedDto[0].Name);
            Assert.Equal(dto[0].BudgetIds.Count, modifiedDto[0].BudgetIds.Count);
            Assert.Contains(scenarioBudget.Id, modifiedDto[0].BudgetIds);
        }

        [Fact]
        public void ShouldDeleteLibraryData()
        {
            // Arrange
            Setup();
            CreateLibraryTestData();

            // Act
            TestHelper.UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(_testTreatmentLibrary.Id);

            Assert.True(
                !TestHelper.UnitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == _testTreatmentLibrary.Id));
            Assert.True(!TestHelper.UnitOfWork.Context.SelectableTreatment.Any(_ => _.Id == _testTreatment.Id));
            Assert.True(!TestHelper.UnitOfWork.Context.TreatmentCost.Any(_ => _.Id == _testTreatmentCost.Id));
            Assert.True(
                !TestHelper.UnitOfWork.Context.TreatmentConsequence.Any(_ =>
                    _.Id == _testTreatmentConsequence.Id));
        }

        [Fact]
        public void GetTreatmentCostsWithEquationJoinsByLibraryIdAndTreatmentName_ObjectsInDb_GetsTheCost()
        {
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, "treatment");
            var treatmentCost = TreatmentCostTestSetup.ModelForEntityInDb(
                TestHelper.UnitOfWork, treatmentId, treatmentLibraryId);

            var costs = TestHelper.UnitOfWork.TreatmentCostRepo.GetTreatmentCostsWithEquationJoinsByLibraryIdAndTreatmentName(
                treatmentLibraryId, "treatment");

            var foundCost = costs.Single();
            ObjectAssertions.EquivalentExcluding(treatmentCost, foundCost, x => x.CriterionLibrary, x => x.Equation.Id);
        }

        [Fact]
        public void GetSelectableTreatmentByLibraryIdAndName_Does()
        {
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatmentId = Guid.NewGuid();
            var treatmentName = RandomStrings.WithPrefix("treatment");
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, treatmentName);

            var result = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatmentByLibraryIdAndName(treatmentLibraryId, treatmentName);

            treatment.BudgetIds = new List<Guid>();
            ObjectAssertions.EquivalentExcluding(treatment, result, x => x.CriterionLibrary);

        }

        [Fact]
        public void GetSelectableTreatmentByLibraryIdAndName_TreatmentInDbWithDifferentName_DoesNotFind()
        {
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatmentId = Guid.NewGuid();
            var treatmentName = RandomStrings.WithPrefix("treatment");
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, treatmentName);

            var result = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatmentByLibraryIdAndName(treatmentLibraryId, "wrongName");

            Assert.Null(result);
        }
    }
}
