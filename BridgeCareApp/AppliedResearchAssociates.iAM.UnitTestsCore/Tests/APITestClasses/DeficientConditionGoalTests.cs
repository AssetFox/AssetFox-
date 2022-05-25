using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class DeficientConditionGoalTests
    {
        private readonly TestHelper _testHelper;
        private readonly DeficientConditionGoalController _controller;

        private static readonly Guid DeficientConditionGoalLibraryId = Guid.Parse("569618ce-ee50-45de-99ce-cd4625134d07");
        private static readonly Guid DeficientConditionGoalId = Guid.Parse("c148ab58-8b27-40c0-a4a4-84454022d032");

        public DeficientConditionGoalTests()
        {
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.SetupDefaultHttpContext();
            }
            _controller = new DeficientConditionGoalController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
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
            if (!_testHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Any())
            {
                _testHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Add(TestDeficientConditionGoalLibrary);
            }
            if (!_testHelper.UnitOfWork.Context.DeficientConditionGoal.Any())
            {
                var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
                TestDeficientConditionGoal.AttributeId = attribute.Id;
                _testHelper.UnitOfWork.Context.DeficientConditionGoal.Add(TestDeficientConditionGoal);
                _testHelper.UnitOfWork.Context.SaveChanges();
            }
        }

        private CriterionLibraryEntity SetupForUpsertOrDelete()
        {
            SetupForGet();
            var criterionLibrary = _testHelper.TestCriterionLibrary();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return criterionLibrary;
        }
        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            // Act
            var result = await _controller.DeficientConditionGoalLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            // Act
            var result = await _controller
                .UpsertDeficientConditionGoalLibrary(TestDeficientConditionGoalLibrary.ToDto());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Act
            var result = await _controller.DeleteDeficientConditionGoalLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllDeficientConditionGoalLibrariesWithDeficientConditionGoals()
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

        [Fact(Skip = "Usual whine about the test being already broken and timers not working")]
        public async Task ShouldModifyDeficientConditionGoalData()
        {
            // Arrange
            var criterionLibrary = SetupForUpsertOrDelete();
            var getResult = await _controller.DeficientConditionGoalLibraries();
            var dtos = (List<DeficientConditionGoalLibraryDTO>)Convert.ChangeType(
                (getResult as OkObjectResult).Value, typeof(List<DeficientConditionGoalLibraryDTO>));

            var dto = dtos[0];
            dto.Description = "Updated Description";
            dto.DeficientConditionGoals[0].Name = "Updated Name";
            dto.DeficientConditionGoals[0].CriterionLibrary =
                criterionLibrary.ToDto();

            // Act
            await _controller.UpsertDeficientConditionGoalLibrary(dto);

            // Assert

            var modifiedDto = _testHelper.UnitOfWork.DeficientConditionGoalRepo
                .GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()[0];
            Assert.Equal(dto.Description, modifiedDto.Description);
            Assert.Single(modifiedDto.AppliedScenarioIds);
            //  Assert.Equal(_testHelper.TestSimulation.Id, modifiedDto.AppliedScenarioIds[0]);
            // to fix the above, explicitly create a Simulation somewhere, perhaps in setup, then use its id and check against said id in the assert.

            Assert.Equal(dto.DeficientConditionGoals[0].Name, modifiedDto.DeficientConditionGoals[0].Name);
            Assert.Equal(dto.DeficientConditionGoals[0].CriterionLibrary.Id,
                modifiedDto.DeficientConditionGoals[0].CriterionLibrary.Id);
            Assert.Equal(dto.DeficientConditionGoals[0].Attribute,
                modifiedDto.DeficientConditionGoals[0].Attribute);
        }

        [Fact]
        public async Task ShouldDeleteDeficientConditionGoalData()
        {
            // Arrange
            var criterionLibrary = SetupForUpsertOrDelete();
            var getResult = await _controller.DeficientConditionGoalLibraries();
            var dtos = (List<DeficientConditionGoalLibraryDTO>)Convert.ChangeType(
                (getResult as OkObjectResult).Value, typeof(List<DeficientConditionGoalLibraryDTO>));

            var deficientConditionGoalLibraryDTO = dtos[0];
            deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].CriterionLibrary =
               criterionLibrary.ToDto();

            await _controller.UpsertDeficientConditionGoalLibrary(
                deficientConditionGoalLibraryDTO);

            // Act
            var result = await _controller.DeleteDeficientConditionGoalLibrary(DeficientConditionGoalLibraryId);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(!_testHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == DeficientConditionGoalLibraryId));
            Assert.True(!_testHelper.UnitOfWork.Context.DeficientConditionGoal.Any(_ => _.Id == DeficientConditionGoalId));
            Assert.True(
                !_testHelper.UnitOfWork.Context.CriterionLibraryDeficientConditionGoal.Any(_ =>
                    _.DeficientConditionGoalId == DeficientConditionGoalId));
        }
    }
}
