﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Library_API_Test_Classes
{
    public class TargetConditionGoalTests
    {
        private readonly TestHelper _testHelper;
        private readonly TargetConditionGoalController _controller;

        private static readonly Guid TargetConditionGoalLibraryId = Guid.Parse("a353d18d-cacf-48c9-b8a3-a58cb7410e81");
        private static readonly Guid TargetConditionGoalId = Guid.Parse("42b3bbfc-d590-4d3d-aea9-fc8221210c57");

        public TargetConditionGoalTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new TargetConditionGoalController(_testHelper.MockEsecSecurity, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object);
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
                    .UpsertTargetConditionGoalLibrary(Guid.Empty, TestTargetConditionGoalLibrary.ToDto());

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
                await _controller.UpsertTargetConditionGoalLibrary(_testHelper.TestSimulation.Id, dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto = _testHelper.UnitOfWork.TargetConditionGoalRepo
                        .TargetConditionGoalLibrariesWithTargetConditionGoals()[0];
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

                await _controller.UpsertTargetConditionGoalLibrary(_testHelper.TestSimulation.Id,
                    targetConditionGoalLibraryDTO);

                // Act
                var result = await _controller.DeleteTargetConditionGoalLibrary(TargetConditionGoalLibraryId);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(!_testHelper.UnitOfWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == TargetConditionGoalLibraryId));
                Assert.True(!_testHelper.UnitOfWork.Context.TargetConditionGoal.Any(_ => _.Id == TargetConditionGoalId));
                Assert.True(!_testHelper.UnitOfWork.Context.TargetConditionGoalLibrarySimulation.Any(_ =>
                    _.TargetConditionGoalLibraryId == TargetConditionGoalLibraryId));
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
    }
}