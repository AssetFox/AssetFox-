using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class DeficientConditionGoalTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;

        private static readonly Guid DeficientConditionGoalLibraryId = Guid.Parse("569618ce-ee50-45de-99ce-cd4625134d07");
        private static readonly Guid DeficientConditionGoalId = Guid.Parse("c148ab58-8b27-40c0-a4a4-84454022d032");
        private static readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private static DeficientConditionGoalController Setup()
        {
            _testHelper.CreateSingletons();
            var controller = new DeficientConditionGoalController(_testHelper.MockEsecSecurityAdmin.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object, _mockClaimHelper.Object);
            return controller;
        }
        private DeficientConditionGoalController CreateTestController(List<string> uClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string claimstr in uClaims)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimstr);
                claims.Add(claim);
            }
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var controller = new DeficientConditionGoalController(_testHelper.MockEsecSecurityAdmin.Object,_testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object,_mockClaimHelper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
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
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
            return criterionLibrary;
        }
        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            var controller = Setup();

            // Act
            var result = await controller.DeficientConditionGoalLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            var controller = Setup();

            // Act
            var result = await controller
                .UpsertDeficientConditionGoalLibrary(TestDeficientConditionGoalLibrary.ToDto());

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            var controller = Setup();

            // Act
            var result = await controller.DeleteDeficientConditionGoalLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllDeficientConditionGoalLibrariesWithDeficientConditionGoals()
        {
            // Arrange
            var controller = Setup();
            SetupForGet();

            // Act
            var result = await controller.DeficientConditionGoalLibraries();

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

        [Fact]
        public async Task ShouldModifyDeficientConditionGoalData()
        {
            // Arrange
            var controller = Setup();
            var criterionLibrary = SetupForUpsertOrDelete();
            var getResult = await controller.DeficientConditionGoalLibraries();
            var dtos = (List<DeficientConditionGoalLibraryDTO>)Convert.ChangeType(
                (getResult as OkObjectResult).Value, typeof(List<DeficientConditionGoalLibraryDTO>));

            var dto = dtos[0];
            dto.Description = "Updated Description";
            dto.DeficientConditionGoals[0].Name = "Updated Name";
            dto.DeficientConditionGoals[0].CriterionLibrary =
                criterionLibrary.ToDto();

            // Act
            await controller.UpsertDeficientConditionGoalLibrary(dto);

            // Assert

            var modifiedDto = _testHelper.UnitOfWork.DeficientConditionGoalRepo
                .GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()[0];
            Assert.Equal(dto.Description, modifiedDto.Description);
            Assert.Equal(dto.DeficientConditionGoals[0].Attribute,
                modifiedDto.DeficientConditionGoals[0].Attribute);
            // below asserts all fail as of 6/6/2022:
            //Assert.Single(modifiedDto.AppliedScenarioIds);
            //  Assert.Equal(_testHelper.TestSimulation.Id, modifiedDto.AppliedScenarioIds[0]);
            // to fix the above, explicitly create a Simulation somewhere, perhaps in setup, then use its id and check against said id in the assert.

            //Assert.Equal(dto.DeficientConditionGoals[0].Name, modifiedDto.DeficientConditionGoals[0].Name);
            //Assert.Equal(dto.DeficientConditionGoals[0].CriterionLibrary.Id,
            //   modifiedDto.DeficientConditionGoals[0].CriterionLibrary.Id);
        }

        [Fact]
        public async Task ShouldDeleteDeficientConditionGoalData()
        {
            // Arrange
            var controller = Setup();
            var criterionLibrary = SetupForUpsertOrDelete();
            var getResult = await controller.DeficientConditionGoalLibraries();
            var dtos = (List<DeficientConditionGoalLibraryDTO>)Convert.ChangeType(
                (getResult as OkObjectResult).Value, typeof(List<DeficientConditionGoalLibraryDTO>));

            var deficientConditionGoalLibraryDTO = dtos[0];
            deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].CriterionLibrary =
               criterionLibrary.ToDto();

            await controller.UpsertDeficientConditionGoalLibrary(
                deficientConditionGoalLibraryDTO);

            // Act
            var result = await controller.DeleteDeficientConditionGoalLibrary(DeficientConditionGoalLibraryId);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(!_testHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == DeficientConditionGoalLibraryId));
            Assert.True(!_testHelper.UnitOfWork.Context.DeficientConditionGoal.Any(_ => _.Id == DeficientConditionGoalId));
            Assert.True(
                !_testHelper.UnitOfWork.Context.CriterionLibraryDeficientConditionGoal.Any(_ =>
                    _.DeficientConditionGoalId == DeficientConditionGoalId));
        }
        [Fact]
        public async Task UserIsDeficientConditionGoalViewFromLibraryAuthorized()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewDeficientConditionGoalFromlLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.DeficientConditionGoalViewAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.DeficientConditionGoalViewPermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Editor));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewDeficientConditionGoalFromlLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsDeficientConditionGoalModifyFromScenarioAuthorized()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyDeficientConditionGoalFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.DeficientConditionGoalModifyAnyFromScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.DeficientConditionGoalModifyPermittedFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyDeficientConditionGoalFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);            
        }
        [Fact]
        public async Task UserIsDeficientConditionGoalModifyFromLibraryAuthorized()
        {
            // Arrange
            var authorizationService = _testHelper.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyDeficientConditionGoalFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.DeficientConditionGoalModifyPermittedFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.DeficientConditionGoalModifyAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, BridgeCareCore.Security.SecurityConstants.Role.ReadOnly));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyDeficientConditionGoalFromLibrary);
            // Assert
            Assert.False(allowed.Succeeded);
        }
    }
}
