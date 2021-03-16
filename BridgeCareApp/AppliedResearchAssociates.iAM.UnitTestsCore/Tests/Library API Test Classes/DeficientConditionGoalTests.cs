using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Library_API_Test_Classes
{
    public class DeficientConditionGoalTests
    {
        private readonly TestHelper _testHelper;
        private readonly DeficientConditionGoalController _controller;

        private static readonly Guid DeficientConditionGoalLibraryId = Guid.Parse("569618ce-ee50-45de-99ce-cd4625134d07");
        private static readonly Guid DeficientConditionGoalId = Guid.Parse("c148ab58-8b27-40c0-a4a4-84454022d032");

        public DeficientConditionGoalTests()
        {
            _testHelper = new TestHelper("IAMv2dcg");
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new DeficientConditionGoalController(_testHelper.UnitOfDataPersistenceWork, _testHelper.MockEsecSecurity);
        }

        public DeficientConditionGoalLibraryEntity TestDeficientConditionGoalLibrary { get; } = new DeficientConditionGoalLibraryEntity
        {
            Id = DeficientConditionGoalLibraryId,
            Name = "Test Name"
        };

        public DeficientConditionGoalEntity TestDeficientConditionGoal { get; } = new DeficientConditionGoalEntity
        {
            Id = DeficientConditionGoalId,
            DeficientConditionGoalLibraryId = DeficientConditionGoalLibraryId,
            Name = "Test Name",
            AllowedDeficientPercentage = 100,
            DeficientLimit = 1.0
        };

        private void SetupForGet()
        {
            _testHelper.UnitOfDataPersistenceWork.Context.DeficientConditionGoalLibrary.Add(TestDeficientConditionGoalLibrary);
            var attribute = _testHelper.UnitOfDataPersistenceWork.Context.Attribute.First();
            TestDeficientConditionGoal.AttributeId = attribute.Id;
            _testHelper.UnitOfDataPersistenceWork.Context.DeficientConditionGoal.Add(TestDeficientConditionGoal);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGet();
            _testHelper.UnitOfDataPersistenceWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.DeficientConditionGoalLibraries();

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
                    .UpsertDeficientConditionGoalLibrary(Guid.Empty, TestDeficientConditionGoalLibrary.ToDto());

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
                var result = await _controller.DeleteDeficientConditionGoalLibrary(Guid.Empty);

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
        public async void ShouldGetAllDeficientConditionGoalLibrariesWithDeficientConditionGoals()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = await _controller.DeficientConditionGoalLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<DeficientConditionGoalLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<DeficientConditionGoalLibraryDTO>));
                Assert.Single(dtos);

                Assert.Equal(DeficientConditionGoalLibraryId, dtos[0].Id);
                Assert.Single(dtos[0].DeficientConditionGoals);

                Assert.Equal(DeficientConditionGoalId, dtos[0].DeficientConditionGoals[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyDeficientConditionGoalData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.DeficientConditionGoalLibraries();
                var dtos = (List<DeficientConditionGoalLibraryDTO>)Convert.ChangeType(
                    (getResult as OkObjectResult).Value, typeof(List<DeficientConditionGoalLibraryDTO>));

                var deficientConditionGoalLibraryDTO = dtos[0];
                deficientConditionGoalLibraryDTO.Description = "Updated Description";
                deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].Name = "Updated Name";
                deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                // Act
                var result = await _controller.UpsertDeficientConditionGoalLibrary(_testHelper.TestSimulation.Id,
                    deficientConditionGoalLibraryDTO);

                // Assert
                Assert.IsType<OkResult>(result);

                var deficientConditionGoalLibraryEntity = _testHelper.UnitOfDataPersistenceWork.Context.DeficientConditionGoalLibrary
                    .Include(_ => _.DeficientConditionGoals)
                    .ThenInclude(_ => _.CriterionLibraryDeficientConditionGoalJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.DeficientConditionGoals)
                    .ThenInclude(_ => _.Attribute)
                    .Include(_ => _.DeficientConditionGoalLibrarySimulationJoins)
                    .Single(_ => _.Id == DeficientConditionGoalLibraryId);

                Assert.Equal(deficientConditionGoalLibraryDTO.Description, deficientConditionGoalLibraryEntity.Description);
                Assert.Single(deficientConditionGoalLibraryEntity.DeficientConditionGoalLibrarySimulationJoins);
                var budgetPriorityLibrarySimulationJoin =
                    deficientConditionGoalLibraryEntity.DeficientConditionGoalLibrarySimulationJoins.ToList()[0];
                Assert.Equal(_testHelper.TestSimulation.Id, budgetPriorityLibrarySimulationJoin.SimulationId);
                var deficientConditionGoalEntity = deficientConditionGoalLibraryEntity.DeficientConditionGoals.ToList()[0];
                Assert.Equal(deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].Name, deficientConditionGoalEntity.Name);
                Assert.NotNull(deficientConditionGoalEntity.CriterionLibraryDeficientConditionGoalJoin);
                Assert.Equal(deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].CriterionLibrary.Id,
                    deficientConditionGoalEntity.CriterionLibraryDeficientConditionGoalJoin.CriterionLibrary.Id);
                Assert.NotNull(deficientConditionGoalEntity.Attribute);
                Assert.Equal(deficientConditionGoalEntity.Attribute.Name,
                    deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].Attribute);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeleteDeficientConditionGoalData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.DeficientConditionGoalLibraries();
                var dtos = (List<DeficientConditionGoalLibraryDTO>)Convert.ChangeType(
                    (getResult as OkObjectResult).Value, typeof(List<DeficientConditionGoalLibraryDTO>));

                var deficientConditionGoalLibraryDTO = dtos[0];
                deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                await _controller.UpsertDeficientConditionGoalLibrary(_testHelper.TestSimulation.Id,
                    deficientConditionGoalLibraryDTO);

                // Act
                var result = await _controller.DeleteDeficientConditionGoalLibrary(DeficientConditionGoalLibraryId);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == DeficientConditionGoalLibraryId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.DeficientConditionGoal.Any(_ => _.Id == DeficientConditionGoalId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.DeficientConditionGoalLibrarySimulation.Any(_ =>
                    _.DeficientConditionGoalLibraryId == DeficientConditionGoalLibraryId));
                Assert.True(
                    !_testHelper.UnitOfDataPersistenceWork.Context.CriterionLibraryDeficientConditionGoal.Any(_ =>
                        _.DeficientConditionGoalId == DeficientConditionGoalId));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
