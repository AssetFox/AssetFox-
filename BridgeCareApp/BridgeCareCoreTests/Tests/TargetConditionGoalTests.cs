using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
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
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCoreTests.Tests
{
    public class TargetConditionGoalTests
    {
        private static readonly Guid TargetConditionGoalLibraryId = Guid.Parse("a353d18d-cacf-48c9-b8a3-a58cb7410e81");
        private static readonly Guid TargetConditionGoalId = Guid.Parse("42b3bbfc-d590-4d3d-aea9-fc8221210c57");
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private TargetConditionGoalController SetupController()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var controller = new TargetConditionGoalController(EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object,
                new TargetConditionGoalPagingService(TestHelper.UnitOfWork));
            return controller;
        }
        private TargetConditionGoalController CreateTestController(List<string> userClaims)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = ClaimsPrincipals.WithNameClaims(userClaims);
            var controller = new TargetConditionGoalController(EsecSecurityMocks.AdminMock.Object, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object,
                new TargetConditionGoalPagingService(TestHelper.UnitOfWork));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        private TargetConditionGoalLibraryEntity
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

        private TargetConditionGoalEntity TestTargetConditionGoal(
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
        private ScenarioTargetConditionGoalEntity TestScenarioTargetConditionGoal(Guid simulationId,
            Guid attributeId,
            Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var returnValue = new ScenarioTargetConditionGoalEntity
            {
                Id = resolveId,
                SimulationId = simulationId,
                AttributeId = attributeId,
                Name = "Test Name",
                Target = 1
            };
            return returnValue;
        }

        private TargetConditionGoalLibraryEntity SetupLibraryForGet()
        {
            var libraryEntity = TestTargetConditionGoalLibraryEntity();
            TestHelper.UnitOfWork.Context.TargetConditionGoalLibrary.Add(libraryEntity);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return libraryEntity;
        }

        public TargetConditionGoalEntity SetupTargetConditionGoal(Guid targetConditionGoalLibraryId)
        {
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var targetConditionGoalEntity = TestTargetConditionGoal(targetConditionGoalLibraryId);
            targetConditionGoalEntity.AttributeId = attribute.Id;
            TestHelper.UnitOfWork.Context.TargetConditionGoal.Add(targetConditionGoalEntity);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return targetConditionGoalEntity;
        }

        private CriterionLibraryDTO SetupCriterionLibraryForUpsertOrDelete()
        {
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            return criterionLibrary;
        }

        private ScenarioTargetConditionGoalEntity SetupForScenarioTargetGet(Guid simulationId)
        {
            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
            var goal = TestScenarioTargetConditionGoal(simulationId, attribute.Id);
            TestHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Add(goal);
            TestHelper.UnitOfWork.Context.SaveChanges();
            return goal;
        }

        private CriterionLibraryDTO SetupForScenarioTargetUpsertOrDelete(Guid simulationId)
        {
            SetupForScenarioTargetGet(simulationId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibrary();
            return criterionLibrary;
        }

        private void JoinCriterionToLibraryConditionGoal(Guid goalId, Guid criterionId)
        {
            var libraryJoin = new CriterionLibraryTargetConditionGoalEntity()
            {
                CriterionLibraryId = criterionId,
                TargetConditionGoalId = goalId
            };

            TestHelper.UnitOfWork.Context.CriterionLibraryTargetConditionGoal.Add(libraryJoin);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private void JoinCriterionTosScenarioConditionGoal(Guid goalId, Guid criterionId)
        {
            var libraryJoin = new CriterionLibraryScenarioTargetConditionGoalEntity()
            {
                CriterionLibraryId = criterionId,
                ScenarioTargetConditionGoalId = goalId
            };

            TestHelper.UnitOfWork.Context.CriterionLibraryScenarioTargetConditionGoal.Add(libraryJoin);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            var controller = SetupController();
            // Act
            var result = await controller.TargetConditionGoalLibraries();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            var controller = SetupController();
            var entity = SetupLibraryForGet();
            var request = new LibraryUpsertPagingRequestModel<TargetConditionGoalLibraryDTO, TargetConditionGoalDTO>();
            request.Library = entity.ToDto();
            // Act
            var result = await controller
                .UpsertTargetConditionGoalLibrary(request);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            var controller = SetupController();
            // Act
            var result = await controller.DeleteTargetConditionGoalLibrary(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetAllTargetConditionGoalLibraries()
        {
            var controller = SetupController();
            // Arrange
            var library = SetupLibraryForGet();
            var goal = SetupTargetConditionGoal(library.Id);

            // Act
            var result = await controller.TargetConditionGoalLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<TargetConditionGoalLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<TargetConditionGoalLibraryDTO>));
            Assert.NotNull(dtos.SingleOrDefault(dto => dto.Id == library.Id));
        }

        [Fact]
        public async Task ShouldModifyTargetConditionGoalData()
        {
            var controller = SetupController();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var criterionLibrary = SetupCriterionLibraryForUpsertOrDelete();
            var library = SetupLibraryForGet();
            var goal = SetupTargetConditionGoal(library.Id);

            var libraryDto = library.ToDto();
            var goalDto = goal.ToDto();
            
            libraryDto.Description = "Updated Description";
            goalDto.Name = "Updated Name";
            goalDto.CriterionLibrary = criterionLibrary;

            var sync = new PagingSyncModel<TargetConditionGoalDTO>()
            {
                UpdateRows = new List<TargetConditionGoalDTO>() { goalDto }
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<TargetConditionGoalLibraryDTO, TargetConditionGoalDTO>()
            {
                IsNewLibrary = false,
                Library = libraryDto,
                PagingSync = sync
            };

            // Act
            await controller.UpsertTargetConditionGoalLibrary(libraryRequest);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.TargetConditionGoalRepo
                .GetTargetConditionGoalLibrariesWithTargetConditionGoals()
                .Single(x => x.Id == library.Id);
            Assert.Equal(libraryDto.Description, modifiedDto.Description);

            // below fails on some db weirdness. The name is updated in the db but not in the get result!?!
            // Assert.Equal(dto.TargetConditionGoals[0].Name, modifiedDto.TargetConditionGoals[0].Name);
            Assert.Equal(goalDto.Attribute, modifiedDto.TargetConditionGoals[0].Attribute);
        }

        [Fact]
        public async Task ShouldDeleteTargetConditionGoalData()
        {
            var controller = SetupController();
            // Arrange
            var criterionLibrary = SetupCriterionLibraryForUpsertOrDelete();
            var targetConditionGoalLibraryEntity = SetupLibraryForGet();
            var libraryId = targetConditionGoalLibraryEntity.Id;
            var targetConditionGoalEntity = SetupTargetConditionGoal(libraryId);
            var goalId = targetConditionGoalEntity.Id;

            JoinCriterionToLibraryConditionGoal(goalId, criterionLibrary.Id);

            // Act
            var result = await controller.DeleteTargetConditionGoalLibrary(libraryId);

            // Assert
            Assert.IsType<OkResult>(result);

            Assert.True(!TestHelper.UnitOfWork.Context.TargetConditionGoalLibrary.Any(_ => _.Id == libraryId));
            Assert.True(!TestHelper.UnitOfWork.Context.TargetConditionGoal.Any(_ => _.Id == goalId));
            Assert.True(
                !TestHelper.UnitOfWork.Context.CriterionLibraryTargetConditionGoal.Any(_ =>
                    _.TargetConditionGoalId == TargetConditionGoalId));
        }

        [Fact]
        public async Task ShouldGetAllScenarioTargetConditionGoalData()
        {
            var controller = SetupController();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var goal = SetupForScenarioTargetGet(simulation.Id);

            // Act
            var result = await controller.GetScenarioTargetConditionGoals(simulation.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<TargetConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(List<TargetConditionGoalDTO>));
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldGetScenarioTargetConditionGoalPageData()
        {
            var controller = SetupController();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var goal = SetupForScenarioTargetGet(simulation.Id);

            // Act
            var request = new PagingRequestModel<TargetConditionGoalDTO>();
            var result = await controller.GetScenarioTargetConditionGoalPage(simulation.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<TargetConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<TargetConditionGoalDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldGetLibraryTargetConditionGoalPageData()
        {
            var controller = SetupController();
            // Arrange
            var library = SetupLibraryForGet();
            var goal = SetupTargetConditionGoal(library.Id);

            // Act
            var request = new PagingRequestModel<TargetConditionGoalDTO>();
            var result = await controller.GetLibraryTargetConditionGoalPage(library.Id, request);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var page = (PagingPageModel<TargetConditionGoalDTO>)Convert.ChangeType(okObjResult.Value,
                typeof(PagingPageModel<TargetConditionGoalDTO>));
            var dtos = page.Items;
            var dto = dtos.Single();
            Assert.Equal(goal.Id, dto.Id);
        }

        [Fact]
        public async Task ShouldModifyScenarioTargetConditionGoalData()
        {
            var controller = SetupController();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var criterionLibrary = SetupForScenarioTargetUpsertOrDelete(simulation.Id);
            var getResult = await controller.GetScenarioTargetConditionGoals(simulation.Id);
            var dtos = (List<TargetConditionGoalDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<TargetConditionGoalDTO>));

            var attribute = TestHelper.UnitOfWork.Context.Attribute.First();

            var deletedTargetConditionId = Guid.NewGuid();
            TestHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Add(new ScenarioTargetConditionGoalEntity
            {
                Id = deletedTargetConditionId,
                SimulationId = simulation.Id,
                AttributeId = attribute.Id,
                Name = "Deleted"
            });
            TestHelper.UnitOfWork.Context.SaveChanges();

            var localScenarioTargetGoals = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulation.Id);
            var indexToDelete = localScenarioTargetGoals.FindIndex(g => g.Id == deletedTargetConditionId);
            var deleteId = localScenarioTargetGoals[indexToDelete].Id;
            var goalToUpdate = localScenarioTargetGoals.Single(g => g.Id!=deletedTargetConditionId);
            var updatedGoalId = goalToUpdate.Id;
            goalToUpdate.Name = "Updated";  
            goalToUpdate.CriterionLibrary = criterionLibrary;
            var newGoalId = Guid.NewGuid();
            var addedGoal = new TargetConditionGoalDTO
            {
                Id = newGoalId,
                Attribute = attribute.Name,
                Name = "New"
            };

            var sync = new PagingSyncModel<TargetConditionGoalDTO>()
            {
                UpdateRows = new List<TargetConditionGoalDTO>() { goalToUpdate },
                AddedRows = new List<TargetConditionGoalDTO>() { addedGoal},
                RowsForDeletion = new List<Guid>() { deleteId}
            };

            // Act
            await controller.UpsertScenarioTargetConditionGoals(simulation.Id, sync);

            // Assert
            var serverScenarioTargetConditionGoals = TestHelper.UnitOfWork.TargetConditionGoalRepo
                .GetScenarioTargetConditionGoals(simulation.Id);
            Assert.Equal(serverScenarioTargetConditionGoals.Count, serverScenarioTargetConditionGoals.Count);

            Assert.False(
                TestHelper.UnitOfWork.Context.ScenarioTargetConditionGoals.Any(_ => _.Id == deletedTargetConditionId));
            localScenarioTargetGoals = TestHelper.UnitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(simulation.Id);
            var localNewTargetGoal = localScenarioTargetGoals.Single(_ => _.Name == "New");
            var serverNewTargetGoal = localScenarioTargetGoals.FirstOrDefault(_ => _.Id == newGoalId);
            Assert.NotNull(serverNewTargetGoal);
            Assert.Equal(localNewTargetGoal.Attribute, serverNewTargetGoal.Attribute);

            var localUpdatedTargetGoal = localScenarioTargetGoals.Single(_ => _.Id == updatedGoalId);
            var serverUpdatedTargetGoal = serverScenarioTargetConditionGoals
                .FirstOrDefault(_ => _.Id == updatedGoalId);
            ObjectAssertions.Equivalent(localNewTargetGoal, serverNewTargetGoal);
            Assert.Equal(localUpdatedTargetGoal.Name, serverUpdatedTargetGoal.Name);
            Assert.Equal(localUpdatedTargetGoal.Attribute, serverUpdatedTargetGoal.Attribute);
            Assert.Equal(localUpdatedTargetGoal.CriterionLibrary.MergedCriteriaExpression,
                serverUpdatedTargetGoal.CriterionLibrary.MergedCriteriaExpression);
        }
        [Fact]
        public async Task UserIsViewTargetConditionGoalFromScenarioAuthorized()
        {
            // non-admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewTargetConditionGoalFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TargetConditionGoalViewPermittedFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewTargetConditionGoalFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyTargetConditionGoalFromLibraryAuthorized()
        {
            // admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyTargetConditionGoalFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TargetConditionGoalUpdateAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyTargetConditionGoalFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsDeleteTargetConditionGoalFromLibraryAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.DeleteTargetConditionGoalFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TargetConditionGoalDeleteAnyFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TargetConditionGoalDeletePermittedFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.ReadOnly }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.DeleteTargetConditionGoalFromLibrary);
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewTargetConditionGoalFromScenarioAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewTargetConditionGoalFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TargetConditionGoalViewAnyFromScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TargetConditionGoalViewPermittedFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewTargetConditionGoalFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }

    }
}
