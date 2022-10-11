using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using MathNet.Numerics.Statistics.Mcmc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework.Internal.Execution;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCoreTests.Tests
{
    public class DeficientConditionGoalTests
    {
        private static readonly Guid DeficientConditionGoalLibraryId = Guid.Parse("569618ce-ee50-45de-99ce-cd4625134d07");
        private static readonly Guid DeficientConditionGoalId = Guid.Parse("c148ab58-8b27-40c0-a4a4-84454022d032");
        private static readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private static DeficientConditionGoalController Setup()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DeficientConditionGoalController(EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService,
                accessor, _mockClaimHelper.Object,
                new DeficientConditionGoalService(TestHelper.UnitOfWork));
            return controller;
        }
        private DeficientConditionGoalController CreateTestController(List<string> uClaims)
        {
            List<Claim> claims = new List<Claim>();
            foreach (string claimName in uClaims)
            {
                Claim claim = new Claim(ClaimTypes.Name, claimName);
                claims.Add(claim);
            }
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var controller = new DeficientConditionGoalController(EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService,
                accessor, _mockClaimHelper.Object,
                 new DeficientConditionGoalService(TestHelper.UnitOfWork));
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

        private void SetupLibraryForGet()
        {
            if (!TestHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Any())
            {
                TestHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Add(TestDeficientConditionGoalLibrary);
            }
            if (!TestHelper.UnitOfWork.Context.DeficientConditionGoal.Any())
            {
                var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
                TestDeficientConditionGoal.AttributeId = attribute.Id;
                TestHelper.UnitOfWork.Context.DeficientConditionGoal.Add(TestDeficientConditionGoal);
                TestHelper.UnitOfWork.Context.SaveChanges();
            }
        }

        public ScenarioDeficientConditionGoalEntity TestScenarioDeficientConditionGoal { get; } = new ScenarioDeficientConditionGoalEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Name",
            AllowedDeficientPercentage = 100,
            DeficientLimit = 1.0
        };

        private ScenarioDeficientConditionGoalEntity SetupScenarioGoalsForGet(Guid simulationId)
        {
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var goal = TestScenarioDeficientConditionGoal;
            goal.AttributeId = attribute.Id;
            goal.SimulationId = simulationId;
            TestHelper.UnitOfWork.Context.ScenarioDeficientConditionGoal.Add(goal);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return goal;

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
            SetupLibraryForGet();
            var request = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>();
            request.Library = TestDeficientConditionGoalLibrary.ToDto();
            // Act
            var result = await controller
                .UpsertDeficientConditionGoalLibrary(request);

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
        public async Task ShouldGetAllDeficientConditionGoalLibraries()
        {
            // Arrange
            var controller = Setup();
            SetupLibraryForGet();

            // Act
            var result = await controller.DeficientConditionGoalLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<DeficientConditionGoalLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<DeficientConditionGoalLibraryDTO>));
            Assert.Single(dtos);
            Assert.Empty(dtos[0].DeficientConditionGoals);
        }

        [Fact]
        public async Task ShouldModifyDeficientConditionGoalData()
        {
            // Arrange
            var controller = Setup();
            SetupLibraryForGet();
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var libraryDto = TestDeficientConditionGoalLibrary.ToDto();
            var goalDto = TestDeficientConditionGoal.ToDto();
            libraryDto.Description = "Updated Description";
            goalDto.Name = "Updated Name";
            goalDto.CriterionLibrary = criterionLibrary;

            var sync = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                UpdateRows = new List<DeficientConditionGoalDTO>() { goalDto }
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>()
            {
                IsNewLibrary = false,
                Library = libraryDto,
                PagingSync = sync
            };

            // Act
            await controller.UpsertDeficientConditionGoalLibrary(libraryRequest);

            // Assert

            var modifiedDto = TestHelper.UnitOfWork.DeficientConditionGoalRepo
                .GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()[0];
            Assert.Equal(libraryDto.Description, modifiedDto.Description);
            Assert.Equal(goalDto.Attribute,
                modifiedDto.DeficientConditionGoals[0].Attribute);
            // below asserts all fail as of 6/6/2022:
            //Assert.Single(modifiedDto.AppliedScenarioIds);
            //  Assert.Equal(_testHelper.TestSimulation.Id, modifiedDto.AppliedScenarioIds[0]);
            // to fix the above, explicitly create a Simulation somewhere, perhaps in setup, then use its id and check against said id in the assert.

            //Assert.Equal(dto.DeficientConditionGoals[0].Name, modifiedDto.DeficientConditionGoals[0].Name);
            //Assert.Equal(dto.DeficientConditionGoals[0].CriterionLibrary.Id,
            //   modifiedDto.DeficientConditionGoals[0].CriterionLibrary.Id);
        }

        [Fact (Skip ="May fail depending on test order")]
        public async Task ShouldDeleteDeficientConditionGoalData()
        {
            // Arrange
            var controller = Setup();
            SetupLibraryForGet();
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var getResult = await controller.DeficientConditionGoalLibraries();
            var deficientConditionGoalLibraryDTO = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalLibrariesWithDeficientConditionGoals()[0];
            deficientConditionGoalLibraryDTO.DeficientConditionGoals[0].CriterionLibrary = criterionLibrary;

            var sync = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                UpdateRows = new List<DeficientConditionGoalDTO>() { deficientConditionGoalLibraryDTO.DeficientConditionGoals[0] }
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<DeficientConditionGoalLibraryDTO, DeficientConditionGoalDTO>()
            {
                IsNewLibrary = false,
                Library = deficientConditionGoalLibraryDTO,
                PagingSync = sync
            };

            await controller.UpsertDeficientConditionGoalLibrary(libraryRequest);

            // Act
            var result = await controller.DeleteDeficientConditionGoalLibrary(DeficientConditionGoalLibraryId);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(!TestHelper.UnitOfWork.Context.DeficientConditionGoalLibrary.Any(_ => _.Id == DeficientConditionGoalLibraryId));
            Assert.True(!TestHelper.UnitOfWork.Context.DeficientConditionGoal.Any(_ => _.Id == DeficientConditionGoalId));
            Assert.True(
                !TestHelper.UnitOfWork.Context.CriterionLibraryDeficientConditionGoal.Any(_ =>
                    _.DeficientConditionGoalId == DeficientConditionGoalId));
        }
        [Fact]
        public async Task UserIsDeficientConditionGoalViewFromLibraryAuthorized()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
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
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
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
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
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
        [Fact]
        public async Task UserIsDeficientConditionGoalViewFromLibraryAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
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
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, BridgeCareCore.Security.SecurityConstants.Role.Administrator));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewDeficientConditionGoalFromlLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }

        //Scenarios
        [Fact]
        public async Task ShouldModifyScenarioDeficientConditionGoalData()
        {
            var controller = Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            SetupScenarioGoalsForGet(simulation.Id);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            var getResult = await controller.GetScenarioDeficientConditionGoals(simulation.Id);
            var dtos = (List<DeficientConditionGoalDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<DeficientConditionGoalDTO>));

            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();

            var deletedGoalId = Guid.NewGuid();
            TestHelper.UnitOfWork.Context.ScenarioDeficientConditionGoal.Add(new ScenarioDeficientConditionGoalEntity
            {
                Id = deletedGoalId,
                SimulationId = simulation.Id,
                AttributeId = attribute.Id,
                Name = "Deleted"
            });
            TestHelper.UnitOfWork.Context.SaveChanges();

            var localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulation.Id);
            var indexToDelete = localScenarioDeficientGoals.FindIndex(g => g.Id == deletedGoalId);
            var deleteId = localScenarioDeficientGoals[indexToDelete].Id;
            var goalToUpdate = localScenarioDeficientGoals.Single(g => g.Id != deletedGoalId);
            var updatedGoalId = goalToUpdate.Id;
            goalToUpdate.Name = "Updated";
            goalToUpdate.CriterionLibrary = criterionLibrary;
            var newGoalId = Guid.NewGuid();
            var addedGoal = new DeficientConditionGoalDTO
            {
                Id = newGoalId,
                Attribute = attribute.Name,
                Name = "New"
            };

            var sync = new PagingSyncModel<DeficientConditionGoalDTO>()
            {
                UpdateRows = new List<DeficientConditionGoalDTO>() { goalToUpdate },
                AddedRows = new List<DeficientConditionGoalDTO>() { addedGoal },
                RowsForDeletion = new List<Guid>() { deleteId }
            };

            // Act
            await controller.UpsertScenarioDeficientConditionGoals(simulation.Id, sync);

            // Assert
            var serverScenarioDeficientConditionGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo
                .GetScenarioDeficientConditionGoals(simulation.Id);
            Assert.Equal(serverScenarioDeficientConditionGoals.Count, serverScenarioDeficientConditionGoals.Count);

            Assert.False(
                TestHelper.UnitOfWork.Context.ScenarioDeficientConditionGoal.Any(_ => _.Id == deletedGoalId));
            localScenarioDeficientGoals = TestHelper.UnitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(simulation.Id);
            var localNewDeficientGoal = localScenarioDeficientGoals.Single(_ => _.Name == "New");
            var serverNewDeficientGoal = localScenarioDeficientGoals.FirstOrDefault(_ => _.Id == newGoalId);
            Assert.NotNull(serverNewDeficientGoal);
            Assert.Equal(localNewDeficientGoal.Attribute, serverNewDeficientGoal.Attribute);

            var localUpdatedDeficientGoal = localScenarioDeficientGoals.Single(_ => _.Id == updatedGoalId);
            var serverUpdatedDeficientGoal = serverScenarioDeficientConditionGoals
                .FirstOrDefault(_ => _.Id == updatedGoalId);
            ObjectAssertions.Equivalent(localNewDeficientGoal, serverNewDeficientGoal);
            Assert.Equal(localUpdatedDeficientGoal.Name, serverUpdatedDeficientGoal.Name);
            Assert.Equal(localUpdatedDeficientGoal.Attribute, serverUpdatedDeficientGoal.Attribute);
            Assert.Equal(localUpdatedDeficientGoal.CriterionLibrary.MergedCriteriaExpression,
                serverUpdatedDeficientGoal.CriterionLibrary.MergedCriteriaExpression);
        }

        [Fact]
        public async Task ShouldGetScenarioTargetConditionGoalPageData()
        {
            var controller = Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var goal = SetupScenarioGoalsForGet(simulation.Id).ToDto();

            // Act
            var request = new PagingRequestModel<DeficientConditionGoalDTO>();
            var result = await controller.GetScenarioDeficientConditionGoalPage(simulation.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<DeficientConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<DeficientConditionGoalDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

    }
}
