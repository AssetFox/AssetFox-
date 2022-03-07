using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
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
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _controller = new TargetConditionGoalController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        public TargetConditionGoalLibraryEntity TestTargetConditionGoalLibrary { get; } = new TargetConditionGoalLibraryEntity
        {
            Id = TargetConditionGoalLibraryId,
            Name = "Test Name"
        };

        public TargetConditionGoalEntity TestTargetConditionGoal { get; } = new TargetConditionGoalEntity
        {
            Id = TargetConditionGoalId,
            TargetConditionGoalLibraryId = TargetConditionGoalLibraryId,
            Name = "Test Name",
            Target = 1
        };
        public ScenarioTargetConditionGoalEntity TestScenarioTargetConditionGoal { get; } = new ScenarioTargetConditionGoalEntity
        {
            Id = ScenarioTargetConditionGoalId,
            Name = "Test Name",
            Target = 1
        };

        private void SetupForGet()
        {
            _testHelper.UnitOfWork.Context.TargetConditionGoalLibrary.Add(TestTargetConditionGoalLibrary);
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
            TestTargetConditionGoal.AttributeId = attribute.Id;
            _testHelper.UnitOfWork.Context.TargetConditionGoal.Add(TestTargetConditionGoal);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGet();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForScenarioTargetGet()
        {
            TestScenarioTargetConditionGoal.SimulationId = _testHelper.TestSimulation.Id;
            TestScenarioTargetConditionGoal.AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id;
            _testHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Add(TestScenarioTargetConditionGoal);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForScenarioTargetUpsertOrDelete()
        {
            SetupForScenarioTargetGet();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.TargetConditionGoalLibraries();

                // Assert
                Assert.IsType<OkObjectResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnPost()
        {
            try
            {
                // Act
                var result = await _controller
                    .UpsertTargetConditionGoalLibrary(TestTargetConditionGoalLibrary.ToDto());

                // Assert
                Assert.IsType<OkResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnDelete()
        {
            try
            {
                // Act
                var result = await _controller.DeleteTargetConditionGoalLibrary(Guid.Empty);

                // Assert
                Assert.IsType<OkResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetAllTargetConditionGoalLibrariesWithTargetConditionGoals()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = await _controller.TargetConditionGoalLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<TargetConditionGoalLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<TargetConditionGoalLibraryDTO>));
                Assert.Single(dtos);

                Assert.Equal(TargetConditionGoalLibraryId, dtos[0].Id);
                Assert.Single(dtos[0].TargetConditionGoals);

                Assert.Equal(TargetConditionGoalId, dtos[0].TargetConditionGoals[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyTargetConditionGoalData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.TargetConditionGoalLibraries();
                var dtos = (List<TargetConditionGoalLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<TargetConditionGoalLibraryDTO>));

                var dto = dtos[0];
                dto.Description = "Updated Description";
                dto.TargetConditionGoals[0].Name = "Updated Name";
                dto.TargetConditionGoals[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                // Act
                await _controller.UpsertTargetConditionGoalLibrary(dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto = _testHelper.UnitOfWork.TargetConditionGoalRepo
                        .GetTargetConditionGoalLibrariesWithTargetConditionGoals()[0];
                    Assert.Equal(dto.Description, modifiedDto.Description);
                    Assert.Single(modifiedDto.AppliedScenarioIds);
                    Assert.Equal(_testHelper.TestSimulation.Id, modifiedDto.AppliedScenarioIds[0]);

                    Assert.Equal(dto.TargetConditionGoals[0].Name, modifiedDto.TargetConditionGoals[0].Name);
                    Assert.Equal(dto.TargetConditionGoals[0].CriterionLibrary.Id,
                        modifiedDto.TargetConditionGoals[0].CriterionLibrary.Id);
                    Assert.Equal(dto.TargetConditionGoals[0].Attribute, modifiedDto.TargetConditionGoals[0].Attribute);
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeleteTargetConditionGoalData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.TargetConditionGoalLibraries();
                var dtos = (List<TargetConditionGoalLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<TargetConditionGoalLibraryDTO>));

                var targetConditionGoalLibraryDTO = dtos[0];
                targetConditionGoalLibraryDTO.TargetConditionGoals[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

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
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetAllScenarioTargetConditionGoalData()
        {
            try
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
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyScenarioTargetConditionGoalData()
        {
            try
            {
                // Arrange
                SetupForScenarioTargetUpsertOrDelete();
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
                localScenarioTargetGoals[0].CriterionLibrary = _testHelper.TestCriterionLibrary.ToDto();
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
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
