﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.Debugging;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class TreatmentTests
    {
        private readonly TestHelper _testHelper;
        private TreatmentController _controller;

        private TreatmentLibraryEntity _testTreatmentLibrary;
        private SelectableTreatmentEntity _testTreatment;
        private TreatmentCostEntity _testTreatmentCost;
        private ConditionalTreatmentConsequenceEntity _testTreatmentConsequence;

        private ScenarioSelectableTreatmentEntity _testScenarioTreatment;
        private ScenarioTreatmentCostEntity _testScenarioTreatmentCost;
        private ScenarioConditionalTreatmentConsequenceEntity _testScenarioTreatmentConsequence;
        private ScenarioBudgetEntity _testScenarioBudget;

        public TreatmentTests()
        {
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.SetupDefaultHttpContext();
            }
        }

        private void CreateAuthorizedController()
        {
            _controller = new TreatmentController(_testHelper.MockTreatmentService.Object, _testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        private void CreateUnauthorizedController()
        {
            _controller = new TreatmentController(_testHelper.MockTreatmentService.Object, _testHelper.MockEsecSecurityNotAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        private void CreateLibraryTestData()
        {
            _testTreatmentLibrary = new TreatmentLibraryEntity { Id = Guid.NewGuid(), Name = "Test Name" };
            _testHelper.UnitOfWork.Context.TreatmentLibrary.Add(_testTreatmentLibrary);

            _testTreatment = new SelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                TreatmentLibraryId = _testTreatmentLibrary.Id,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testTreatment);

            _testTreatmentCost = new TreatmentCostEntity { Id = Guid.NewGuid(), TreatmentId = _testTreatment.Id };
            _testHelper.UnitOfWork.Context.AddEntity(_testTreatmentCost);

            _testTreatmentConsequence = new ConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                SelectableTreatmentId = _testTreatment.Id,
                ChangeValue = "1",
                AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testTreatmentConsequence);

            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void CreateScenarioTestData(Guid simulationId)
        {
            var budget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name"
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioBudget);


            _testScenarioTreatment = new ScenarioSelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatment);
            _testHelper.UnitOfWork.Context.AddEntity(new ScenarioSelectableTreatmentScenarioBudgetEntity
            {
                ScenarioBudgetId = _testScenarioBudget.Id,
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            });

            _testScenarioTreatmentCost = new ScenarioTreatmentCostEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentCost);


            _testScenarioTreatmentConsequence = new ScenarioConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id,
                ChangeValue = "1",
                AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentConsequence);


            _testHelper.UnitOfWork.Context.SaveChanges();
        }


        [Fact]
        public async Task ShouldReturnOkResultOnLibraryGet()
        {
            // Act
            CreateAuthorizedController();
            var result = await _controller.GetTreatmentLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioGet()
        {
            var simulation = _testHelper.CreateSimulation();
            // Act
            CreateAuthorizedController();
            var result = await _controller.GetScenarioSelectedTreatments(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryPost()
        {
            // Arrange
            CreateAuthorizedController();
            var dto = new TreatmentLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Treatments = new List<TreatmentDTO>()
            };

            // Act
            var result = await _controller.UpsertTreatmentLibrary(dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            // Arrange
            CreateAuthorizedController();
            var dtos = new List<TreatmentDTO>();
            var simulation = _testHelper.CreateSimulation();

            // Act
            var result = await _controller.UpsertScenarioSelectedTreatments(simulation.Id, dtos);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryDelete()
        {
            // Arrange
            CreateAuthorizedController();

            // Act
            var result = await _controller.DeleteTreatmentLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetLibraryTreatmentData()
        {
            // Arrange
            CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await _controller.GetTreatmentLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<TreatmentLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<TreatmentLibraryDTO>));
            Assert.True(dtos.Any(t => t.Id == _testTreatmentLibrary.Id));
            var resultTreatmentLibrary = dtos.FirstOrDefault(t => t.Id == _testTreatmentLibrary.Id);
            Assert.Equal(_testTreatmentLibrary.Id, resultTreatmentLibrary.Id);
            Assert.Single(resultTreatmentLibrary.Treatments);

            Assert.Equal(_testTreatment.Id, resultTreatmentLibrary.Treatments[0].Id);
            Assert.Single(resultTreatmentLibrary.Treatments[0].Consequences);
            Assert.Single(resultTreatmentLibrary.Treatments[0].Costs);

            Assert.Equal(_testTreatmentConsequence.Id, resultTreatmentLibrary.Treatments[0].Consequences[0].Id);
            Assert.Equal(_testTreatmentCost.Id, resultTreatmentLibrary.Treatments[0].Costs[0].Id);
        }

        [Fact]
        public async Task ShouldGetScenarioTreatmentData()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);

            // Act
            var result = await _controller.GetScenarioSelectedTreatments(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<TreatmentDTO>)Convert.ChangeType(okObjResult.Value, typeof(List<TreatmentDTO>));
            Assert.Single(dtos);

            Assert.Equal(_testScenarioTreatment.Id, dtos[0].Id);
            Assert.Single(dtos[0].Consequences);
            Assert.Single(dtos[0].Costs);
            Assert.Single(dtos[0].BudgetIds);

            Assert.Equal(_testScenarioTreatmentConsequence.Id, dtos[0].Consequences[0].Id);
            Assert.Equal(_testScenarioTreatmentCost.Id, dtos[0].Costs[0].Id);
            Assert.True(dtos[0].BudgetIds.Contains(_testScenarioBudget.Id));
        }

        [Fact]
        // wjwjwj deleted timer
        public async Task ShouldModifyLibraryTreatmentData()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            using var foo = new ErrorConditionIncrement();
            CreateAuthorizedController();
            CreateLibraryTestData();

            var dto = _testHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries();
            var dtoLibrary = dto.Where(t => t.Name == "Test Name").FirstOrDefault();
            dtoLibrary.Description = "Updated Description";
            dtoLibrary.Treatments[0].Name = "Updated Name";
            dtoLibrary.Treatments[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dtoLibrary.Treatments[0].Costs[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dtoLibrary.Treatments[0].Costs[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            dtoLibrary.Treatments[0].Consequences[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dtoLibrary.Treatments[0].Consequences[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };

            // Act
            await _controller.UpsertTreatmentLibrary(dtoLibrary);

            // Assert
            await Task.Delay(5000);
            var modifiedDto =
                _testHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries()[0];
            Assert.Equal(dtoLibrary.Description, modifiedDto.Description);
            Assert.True(modifiedDto.AppliedScenarioIds.Any());
            Assert.Equal(simulation.Id, modifiedDto.AppliedScenarioIds[0]);

            Assert.Equal(dtoLibrary.Treatments[0].Name, modifiedDto.Treatments[0].Name);
            Assert.Equal(dtoLibrary.Treatments[0].CriterionLibrary.Id,
                modifiedDto.Treatments[0].CriterionLibrary.Id);
            Assert.True(modifiedDto.Treatments[0].Costs.Any());

            Assert.Equal(dtoLibrary.Treatments[0].Costs[0].CriterionLibrary.Id,
                modifiedDto.Treatments[0].Costs[0].CriterionLibrary.Id);
            Assert.Equal(dtoLibrary.Treatments[0].Costs[0].Equation.Id,
                modifiedDto.Treatments[0].Costs[0].Equation.Id);

            Assert.Equal(dtoLibrary.Treatments[0].Costs[0].CriterionLibrary.Id,
                modifiedDto.Treatments[0].Consequences[0].CriterionLibrary.Id);
            Assert.Equal(dtoLibrary.Treatments[0].Consequences[0].Equation.Id,
                modifiedDto.Treatments[0].Consequences[0].Equation.Id);
        }

        [Fact]
        // Wjwjwj created timer
        public async Task ShouldModifyScenarioTreatmentData()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            CreateAuthorizedController();
            CreateScenarioTestData(simulation.Id);

            var scenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = "",
                SimulationId = simulation.Id
            };
            _testHelper.UnitOfWork.Context.AddEntity(scenarioBudget);
            _testHelper.UnitOfWork.Context.SaveChanges();

            var dto = _testHelper.UnitOfWork.SelectableTreatmentRepo
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
            await _controller.UpsertScenarioSelectedTreatments(simulation.Id, dto);

            // Assert
            await Task.Delay(5000);
            var modifiedDto = _testHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);
            Assert.Equal(dto[0].Description, modifiedDto[0].Description);
            Assert.Equal(dto[0].Name, modifiedDto[0].Name);
            Assert.Equal(dto[0].CriterionLibrary.Id, modifiedDto[0].CriterionLibrary.Id);
            Assert.Equal(dto[0].Costs[0].CriterionLibrary.Id, modifiedDto[0].Costs[0].CriterionLibrary.Id);
            Assert.Equal(dto[0].Costs[0].Equation.Id, modifiedDto[0].Costs[0].Equation.Id);
            Assert.Equal(dto[0].Consequences[0].CriterionLibrary.Id,
                modifiedDto[0].Consequences[0].CriterionLibrary.Id);
            Assert.Equal(dto[0].Consequences[0].Equation.Id, modifiedDto[0].Consequences[0].Equation.Id);
            Assert.Equal(dto[0].BudgetIds.Count, modifiedDto[0].BudgetIds.Count);
            Assert.True(modifiedDto[0].BudgetIds.Contains(scenarioBudget.Id));
        }

        [Fact]
        public async Task ShouldThrowUnauthorizedException()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            using var _ = new ErrorConditionIncrement();
            CreateUnauthorizedController();
            CreateScenarioTestData(simulation.Id);

            var dto = _testHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);

            // Act
            var result = await _controller.UpsertScenarioSelectedTreatments(simulation.Id, dto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ShouldDeleteLibraryData()
        {
            // Arrange
            CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await _controller.DeleteTreatmentLibrary(_testTreatmentLibrary.Id);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(
                !_testHelper.UnitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == _testTreatmentLibrary.Id));
            Assert.True(!_testHelper.UnitOfWork.Context.SelectableTreatment.Any(_ => _.Id == _testTreatment.Id));
            Assert.True(!_testHelper.UnitOfWork.Context.TreatmentCost.Any(_ => _.Id == _testTreatmentCost.Id));
            Assert.True(
                !_testHelper.UnitOfWork.Context.TreatmentConsequence.Any(_ =>
                    _.Id == _testTreatmentConsequence.Id));
        }
    }
}
