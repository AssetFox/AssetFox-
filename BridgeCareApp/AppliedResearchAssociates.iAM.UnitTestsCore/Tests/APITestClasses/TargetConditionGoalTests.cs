﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class TargetConditionGoalTests
    {
        private readonly TestHelper _testHelper;
        private readonly TargetConditionGoalController _controller;

        private static readonly Guid TargetConditionGoalLibraryId = Guid.Parse("a353d18d-cacf-48c9-b8a3-a58cb7410e81");
        private static readonly Guid TargetConditionGoalId = Guid.Parse("42b3bbfc-d590-4d3d-aea9-fc8221210c57");
        private static readonly Guid ScenarioTargetConditionGoalId = Guid.Parse("65FA24FD-3FA2-4FB8-94D0-1AC2AED4336E");

        public TargetConditionGoalTests()
        {
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.SetupDefaultHttpContext();
            }
            _controller = new TargetConditionGoalController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        public TargetConditionGoalLibraryEntity
            TestTargetConditionGoalLibraryEntity(
            Guid? id = null,
            string name = null
            )
        {
            var resolvedId = id ?? Guid.NewGuid();
            var resolvedName = name ?? RandomStrings.Length11();
            var returnValue = new TargetConditionGoalLibraryEntity
            {
                Id = resolvedId,
                Name = resolvedName,
            };
            return returnValue;
        }

        public TargetConditionGoalEntity TestTargetConditionGoal(
            Guid libraryId,
            Guid? id = null,
            string name = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var resolveName = name ?? RandomStrings.Length11();
            var returnValue = new TargetConditionGoalEntity
            {
                Id = resolveId,
                TargetConditionGoalLibraryId = libraryId,
                Name = resolveName,
                Target = 1
            };
            return returnValue;
        }
        public ScenarioTargetConditionGoalEntity TestScenarioTargetConditionGoal { get; } = new ScenarioTargetConditionGoalEntity
        {
            Id = ScenarioTargetConditionGoalId,
            Name = "Test Name",
            Target = 1
        };

        private TargetConditionGoalLibraryEntity SetupLibraryForGet()
        {
            var libraryEntity = TestTargetConditionGoalLibraryEntity();
            _testHelper.UnitOfWork.Context.TargetConditionGoalLibrary.Add(libraryEntity);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return libraryEntity;
        }

        public TargetConditionGoalEntity SetupTargetConditionGoal(Guid targetConditionGoalLibraryId)
        {
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
            var targetConditionGoalEntity = TestTargetConditionGoal(targetConditionGoalLibraryId);
            targetConditionGoalEntity.AttributeId = attribute.Id;
            _testHelper.UnitOfWork.Context.TargetConditionGoal.Add(targetConditionGoalEntity);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return targetConditionGoalEntity;
        }

        private CriterionLibraryEntity SetupCriterionLibraryForUpsertOrDelete()
        {
            //var criterionLibraries = _testHelper.UnitOfWork.Context.CriterionLibrary.ToList();
            //_testHelper.UnitOfWork.Context.CriterionLibrary.RemoveRange(criterionLibraries);
            //_testHelper.UnitOfWork.Context.SaveChanges();
            var criterionLibrary = _testHelper.TestCriterionLibrary();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return criterionLibrary;
        }

        private void SetupForScenarioTargetGet()
        {
            if (!_testHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Any())
            {
                TestScenarioTargetConditionGoal.SimulationId = _testHelper.TestSimulation.Id;
                TestScenarioTargetConditionGoal.AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id;
                _testHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Add(TestScenarioTargetConditionGoal);
                _testHelper.UnitOfWork.Context.SaveChanges();
            }
        }

        private CriterionLibraryEntity SetupForScenarioTargetUpsertOrDelete()
        {
            SetupForScenarioTargetGet();
            var criterionLibrary = _testHelper.TestCriterionLibrary();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return criterionLibrary;
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            // Act
            var result = await _controller.TargetConditionGoalLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            var entity = SetupLibraryForGet();
            // Act
            var result = await _controller
                .UpsertTargetConditionGoalLibrary(entity.ToDto());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Act
            var result = await _controller.DeleteTargetConditionGoalLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllTargetConditionGoalLibrariesWithTargetConditionGoals()
        {
            // Arrange
            var library = SetupLibraryForGet();
            var goal = SetupTargetConditionGoal(library.Id);

            // Act
            var result = await _controller.TargetConditionGoalLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<TargetConditionGoalLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<TargetConditionGoalLibraryDTO>));
            var foundLibrary = dtos.Single(dto => dto.Id == library.Id);

            Assert.Equal(goal.Id, foundLibrary.TargetConditionGoals[0].Id);
        }

        [Fact (Skip = "Is broken. Despite appearances, was broken prior to WJ work on getting the tests to be independent. The problem was that the code inside the timer did not fire as the test was already completed.")]
        public async Task ShouldModifyTargetConditionGoalData()
        {
            // Arrange
            var criterionLibraryEntity = SetupCriterionLibraryForUpsertOrDelete();
            var library = SetupLibraryForGet();
            var goal = SetupTargetConditionGoal(library.Id);
            var getResult = await _controller.TargetConditionGoalLibraries();
            var dtos = (List<TargetConditionGoalLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<TargetConditionGoalLibraryDTO>));

            var dto = dtos.Single(l => l.Id == library.Id);
            dto.Description = "Updated Description";
            dto.TargetConditionGoals[0].Name = "Updated Name";
            dto.TargetConditionGoals[0].CriterionLibrary =
                criterionLibraryEntity.ToDto();

            // Act
            await _controller.UpsertTargetConditionGoalLibrary(dto);

            // Assert
            await Task.Delay(5000);
            var modifiedDto = _testHelper.UnitOfWork.TargetConditionGoalRepo
                .GetTargetConditionGoalLibrariesWithTargetConditionGoals()
                .Single(x => x.Id == library.Id);
            Assert.Equal(dto.Description, modifiedDto.Description);
            Assert.Single(modifiedDto.AppliedScenarioIds);
            Assert.Equal(_testHelper.TestSimulation.Id, modifiedDto.AppliedScenarioIds[0]);

            Assert.Equal(dto.TargetConditionGoals[0].Name, modifiedDto.TargetConditionGoals[0].Name);
            Assert.Equal(dto.TargetConditionGoals[0].CriterionLibrary.Id,
                modifiedDto.TargetConditionGoals[0].CriterionLibrary.Id);
            Assert.Equal(dto.TargetConditionGoals[0].Attribute, modifiedDto.TargetConditionGoals[0].Attribute);
        }

        [Fact]
        public async Task ShouldDeleteTargetConditionGoalData()
        {
            // Arrange
            var criterionLibraryEntity = SetupCriterionLibraryForUpsertOrDelete();
            var targetConditionGoalLibraryEntity = SetupLibraryForGet();
            var targetConditionGoalEntity = SetupTargetConditionGoal(targetConditionGoalLibraryEntity.Id);
            var getResult = await _controller.TargetConditionGoalLibraries();
            var dtos = (List<TargetConditionGoalLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<TargetConditionGoalLibraryDTO>));

            var targetConditionGoalLibraryDTO = dtos.Single(dto => dto.Id == targetConditionGoalLibraryEntity.Id);
            targetConditionGoalLibraryDTO.TargetConditionGoals[0].CriterionLibrary =
                criterionLibraryEntity.ToDto();

            await _controller.UpsertTargetConditionGoalLibrary(
                targetConditionGoalLibraryDTO);

            // Act
            var result = await _controller.DeleteTargetConditionGoalLibrary(TargetConditionGoalLibraryId);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(!_testHelper.UnitOfWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == TargetConditionGoalLibraryId));
            Assert.True(!_testHelper.UnitOfWork.Context.TargetConditionGoal.Any(_ => _.Id == TargetConditionGoalId));
            Assert.True(
                !_testHelper.UnitOfWork.Context.CriterionLibraryTargetConditionGoal.Any(_ =>
                    _.TargetConditionGoalId == TargetConditionGoalId));
        }

        [Fact]
        public async Task ShouldGetAllScenarioTargetConditionGoalData()
        {
            // Arrange
            SetupForScenarioTargetGet();

            // Act
            var result = await _controller.GetScenarioTargetConditionGoals(_testHelper.TestSimulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<TargetConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<TargetConditionGoalDTO>));
            Assert.Single(dtos);
            Assert.Equal(TestScenarioTargetConditionGoal.Id, dtos[0].Id);
        }

        [Fact]
        public async Task ShouldModifyScenarioTargetConditionGoalData()
        {
            // Arrange
            var criterionLibraryEntity = SetupForScenarioTargetUpsertOrDelete();
            var getResult = await _controller.GetScenarioTargetConditionGoals(_testHelper.TestSimulation.Id);
            var dtos = (List<TargetConditionGoalDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<TargetConditionGoalDTO>));

            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();

            var deletedTargetConditionId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Add(new ScenarioTargetConditionGoalEntity
            {
                Id = deletedTargetConditionId,
                SimulationId = _testHelper.TestSimulation.Id,
                AttributeId = attribute.Id,
                Name = "Deleted"
            });

            var localScenarioTargetGoals = _testHelper.UnitOfWork.TargetConditionGoalRepo
                .GetScenarioTargetConditionGoals(_testHelper.TestSimulation.Id);
            localScenarioTargetGoals[0].Name = "Updated";
            localScenarioTargetGoals[0].CriterionLibrary = criterionLibraryEntity.ToDto();
            localScenarioTargetGoals.Add(new TargetConditionGoalDTO
            {
                Id = Guid.NewGuid(),
                Attribute = attribute.Name,
                Name = "New"
            });

            // Act
            await _controller.UpsertScenarioTargetConditionGoals(_testHelper.TestSimulation.Id, localScenarioTargetGoals);

            // Assert
            var timer = new Timer { Interval = 5000 };
            timer.Elapsed += delegate
            {
                var serverScenarioTargetConditionGoals = _testHelper.UnitOfWork.TargetConditionGoalRepo
                    .GetScenarioTargetConditionGoals(_testHelper.TestSimulation.Id);
                Assert.Equal(serverScenarioTargetConditionGoals.Count, serverScenarioTargetConditionGoals.Count);

                Assert.True(
                    !_testHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Any(_ => _.Id == deletedTargetConditionId));

                var localNewTargetGoal = localScenarioTargetGoals.Single(_ => _.Name == "New");
                var serverNewTargetGoal = localScenarioTargetGoals.FirstOrDefault(_ => _.Id == localNewTargetGoal.Id);
                Assert.NotNull(serverNewTargetGoal);
                Assert.Equal(localNewTargetGoal.Attribute, serverNewTargetGoal.Attribute);

                var localUpdatedTargetGoal = localScenarioTargetGoals.Single(_ => _.Id == ScenarioTargetConditionGoalId);
                var serverUpdatedTargetGoal = serverScenarioTargetConditionGoals
                    .FirstOrDefault(_ => _.Id == ScenarioTargetConditionGoalId);
                Assert.Equal(localUpdatedTargetGoal.Name, serverNewTargetGoal.Name);
                Assert.Equal(localUpdatedTargetGoal.Attribute, serverNewTargetGoal.Attribute);
                Assert.Equal(localUpdatedTargetGoal.CriterionLibrary.Id, serverNewTargetGoal.CriterionLibrary.Id);
                Assert.Equal(localUpdatedTargetGoal.CriterionLibrary.MergedCriteriaExpression,
                    serverNewTargetGoal.CriterionLibrary.MergedCriteriaExpression);
            };
        }
    }
}
