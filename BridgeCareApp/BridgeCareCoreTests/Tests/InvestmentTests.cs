//using System.Data;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Text;
//using AppliedResearchAssociates;
//using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
//using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
//using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
//using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
//using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
//using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
//using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
//using AppliedResearchAssociates.iAM.DTOs;
//using AppliedResearchAssociates.iAM.DTOs.Enums;
//using AppliedResearchAssociates.iAM.Hubs.Interfaces;
//using AppliedResearchAssociates.iAM.Hubs.Services;
//using AppliedResearchAssociates.iAM.Reporting.Logging;
//using AppliedResearchAssociates.iAM.TestHelpers;
//using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
//using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
//using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
//using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
//using BridgeCareCore.Controllers;
//using BridgeCareCore.Interfaces;
//using BridgeCareCore.Interfaces.DefaultData;
//using BridgeCareCore.Models;
//using BridgeCareCore.Models.DefaultData;
//using BridgeCareCore.Services;
//using BridgeCareCore.Services.DefaultData;
//using BridgeCareCore.Utils;
//using BridgeCareCore.Utils.Interfaces;
//using BridgeCareCoreTests.Helpers;
//using BridgeCareCoreTests.Tests.SecurityUtilsClasses;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Primitives;
//using Microsoft.SqlServer.Dac.Model;
//using Moq;
//using MoreLinq;
//using OfficeOpenXml;
//using Xunit;

//namespace BridgeCareCoreTests.Tests
//{
//    public class InvestmentControllerTests
//    {

//        private IHttpContextAccessor CreateRequestForExceptionTesting(FormFile file = null)
//        {
//            var httpContext = new DefaultHttpContext();

//            FormFileCollection formFileCollection;
//            if (file != null)
//            {
//                formFileCollection = new FormFileCollection { file };
//            }
//            else
//            {
//                formFileCollection = new FormFileCollection();
//            }

//            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFileCollection);
//            var accessor = new Mock<IHttpContextAccessor>();
//            accessor.Setup(_ => _.HttpContext).Returns(httpContext);
//            return accessor.Object;
//        }

//        [Fact]
//        public async Task RequestUpsertNewBudgetLibrary_ForwardsRequestToService()
//        {
//            var user = UserDtos.Admin();
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupLibraryAccessLibraryDoesNotExist(libraryId);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork);
//            var library = new BudgetLibraryDTO
//            {
//                Id = libraryId,
//            };
//            var pagingSync = new InvestmentPagingSyncModel
//            {
                
//            };
//            var upsertRequest = new InvestmentLibraryUpsertPagingRequestModel
//            {
//                IsNewLibrary = true,
//                Library = library,
//                PagingSync = pagingSync,
//            };
//            var _ = await controller.UpsertBudgetLibrary(upsertRequest);
//            var upsertCalls = budgetRepo.GetUpsertBudgetLibraryCalls();
//            var upsertOrDeleteUsersCalls = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.UpsertOrDeleteUsers));
//            Assert.Single(upsertOrDeleteUsersCalls);
//            var actualDto = upsertCalls.Single();
//            ObjectAssertions.Equivalent(library, actualDto);
//        }

//        [Fact]
//        public async Task RequestUpsertExistingBudgetLibrary_ForwardsRequestToService()
//        {
//            var user = UserDtos.Admin();
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, LibraryAccessLevel.Owner);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork);
//            var library = new BudgetLibraryDTO
//            {
//                Id = libraryId,
//            };
//            var pagingSync = new InvestmentPagingSyncModel
//            {

//            };
//            var upsertRequest = new InvestmentLibraryUpsertPagingRequestModel
//            {
//                IsNewLibrary = false,
//                Library = library,
//                PagingSync = pagingSync,
//            };
//            var _ = await controller.UpsertBudgetLibrary(upsertRequest);
//            var upsertCalls = budgetRepo.GetUpsertBudgetLibraryCalls();
//            var actualDto = upsertCalls.Single();
//            ObjectAssertions.Equivalent(library, actualDto);
//        }

//        [Fact]
//        public async Task RequestUpsertExistingBudgetLibrary_UserIsAdminWithoutExplicitAccess_ForwardsRequestToService()
//        {
//            var user = UserDtos.Admin();
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, null);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork);
//            var library = new BudgetLibraryDTO
//            {
//                Id = libraryId,
//            };
//            var pagingSync = new InvestmentPagingSyncModel
//            {

//            };
//            var upsertRequest = new InvestmentLibraryUpsertPagingRequestModel
//            {
//                IsNewLibrary = false,
//                Library = library,
//                PagingSync = pagingSync,
//            };
//            var _ = await controller.UpsertBudgetLibrary(upsertRequest);
//            var upsertCalls = budgetRepo.GetUpsertBudgetLibraryCalls();
//            var actualDto = upsertCalls.Single();
//            ObjectAssertions.Equivalent(library, actualDto);
//        }

//        [Fact]
//        public async Task RequestUpsertExistingBudgetLibrary_UserIsNotAdminWithoutExplicitAccess_DoesNotForwardRequestToService()
//        {
//            var user = UserDtos.Admin();
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, null);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);
//            var library = new BudgetLibraryDTO
//            {
//                Id = libraryId,
//            };
//            var pagingSync = new InvestmentPagingSyncModel
//            {

//            };
//            var upsertRequest = new InvestmentLibraryUpsertPagingRequestModel
//            {
//                IsNewLibrary = false,
//                Library = library,
//                PagingSync = pagingSync,
//            };
//            await controller.UpsertBudgetLibrary(upsertRequest);
//            var upsertCalls = budgetRepo.GetUpsertBudgetLibraryCalls();
//            Assert.Empty(upsertCalls);
//            var messages = hubService.ThreeArgumentUserMessages();
//            var message = messages.Single();
//            Assert.Contains(ClaimHelper.LibraryModifyUnauthorizedMessage, message);
//        }

//        [Fact]
//        public async Task DeleteBudgetLibrary_LibraryDoesNotExistAdminUser_UnauthorizedAndDoesNotCallDeleteOnRepo()
//        {
//            var user = UserDtos.Admin();
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupLibraryAccessLibraryDoesNotExist(libraryId);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork, hubService);

//            var result = await controller.DeleteBudgetLibrary(libraryId);

//            ActionResultAssertions.Ok(result);
//            budgetRepo.Verify(br => br.DeleteBudgetLibrary(It.IsAny<Guid>()), Times.Never());
//            var message = hubService.SingleThreeArgumentUserMessage();
//            Assert.Contains(ClaimHelper.CantDeleteNonexistentLibraryMessage, message);
//        }

//        [Fact]
//        public async Task DeleteBudgetLibrary_LibraryDoesNotExistNonAdminUser_UnauthorizedAndDoesNotCallDeleteOnRepo()
//        {
//            var user = UserDtos.Dbe();
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupLibraryAccessLibraryDoesNotExist(libraryId);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);

//            var result = await controller.DeleteBudgetLibrary(libraryId);

//            ActionResultAssertions.Ok(result);
//            budgetRepo.Verify(br => br.DeleteBudgetLibrary(It.IsAny<Guid>()), Times.Never());
//            var message = hubService.SingleThreeArgumentUserMessage();
//            Assert.Contains(ClaimHelper.CantDeleteNonexistentLibraryMessage, message);
//        }

//        [Fact]
//        public async Task DeleteBudgetLibrary_AdminUser_Ok()
//        {
//            var user = UserDtos.Admin();
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, null);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork);

//            var result = await controller.DeleteBudgetLibrary(libraryId);

//            ActionResultAssertions.Ok(result);
//            var calls = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.DeleteBudgetLibrary));
//        }

//        [Fact]
//        public async Task DeleteBudgetLibrary_OwnerButNotAdminUser_Ok()
//        {
//            var user = UserDtos.Dbe();
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, LibraryAccessLevel.Owner);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork);

//            var result = await controller.DeleteBudgetLibrary(libraryId);

//            ActionResultAssertions.Ok(result);
//            var calls = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.DeleteBudgetLibrary));
//            Assert.Single(calls);
//        }

//        [Fact]
//        public async Task DeleteBudgetLibrary_ModifyPermissionButNotAdminUser_DoesNotDelete()
//        {
//            var user = UserDtos.Dbe();
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibraryAccess(libraryId, user.Id, LibraryAccessLevel.Modify);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);

//            var result = await controller.DeleteBudgetLibrary(libraryId);

//            ActionResultAssertions.Ok(result);
//            var calls = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.DeleteBudgetLibrary));
//            Assert.Empty(calls);
//            var messages = hubService.ThreeArgumentUserMessages();
//            var errorMessage = messages.Single();
//            Assert.Contains(ClaimHelper.LibraryDeleteUnauthorizedMessage, errorMessage);
//        }

//        [Fact]
//        public async Task GetBudgetLibraries_UserIsAdmin_CallsGetLibrariesWithoutChildrenOnRepo()
//        {
//            var user = UserDtos.Admin();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var budgetRepo = BudgetRepositoryMocks.New();
//            var libraryId = Guid.NewGuid();
//            var budgetLibrary = new BudgetLibraryDTO
//            {
//                Id = libraryId,
//            };
//            var budgetLibraries = new List<BudgetLibraryDTO> { budgetLibrary };
//            budgetRepo.Setup(br => br.GetBudgetLibrariesNoChildren()).Returns(budgetLibraries);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork);

//            var libraries = await controller.GetBudgetLibraries();

//            var returnedBudgetLibraries = ActionResultAssertions.OkObject(libraries) as List<BudgetLibraryDTO>;
//            var returnedBudgetLibrary = returnedBudgetLibraries.Single();
//            ObjectAssertions.Equivalent(budgetLibrary, returnedBudgetLibrary);
//        }

//        [Fact]
//        public async Task GetBudgetLibraries_UserIsNotAdmin_CallsGetBudgetLibrariesNoChildrenAccessibleToUser()
//        {
//            var user = UserDtos.Dbe();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var budgetRepo = BudgetRepositoryMocks.New();
//            var libraryId = Guid.NewGuid();
//            var budgetLibrary = new BudgetLibraryDTO
//            {
//                Id = libraryId,
//            };
//            var budgetLibraries = new List<BudgetLibraryDTO> { budgetLibrary };
//            budgetRepo.Setup(br => br.GetBudgetLibrariesNoChildrenAccessibleToUser(user.Id)).Returns(budgetLibraries);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork);

//            var libraries = await controller.GetBudgetLibraries();

//            var returnedBudgetLibraries = ActionResultAssertions.OkObject(libraries) as List<BudgetLibraryDTO>;
//            var returnedBudgetLibrary = returnedBudgetLibraries.Single();
//            ObjectAssertions.Equivalent(budgetLibrary, returnedBudgetLibrary);
//        }

//        [Fact]
//        public async Task GetInvestment_UserIsAdmin_BudgetRepoAndInvestmentPlanRepo()
//        {
//            var user = UserDtos.Admin();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var budgetRepo = BudgetRepositoryMocks.New();
//            var simulationId = Guid.NewGuid();
//            var libraryId = Guid.NewGuid();
//            var budgetId = Guid.NewGuid();
//            var budget = new BudgetDTO
//            {
//                Id = budgetId,
//            };
//            var budgets = new List<BudgetDTO> { budget };
//            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgets);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock();
//            unitOfWork.SetupInvestmentPlanRepo(investmentPlanRepo);
//            var investmentPlanId = Guid.NewGuid();
//            var investmentPlanDto = new InvestmentPlanDTO
//            {
//                Id = investmentPlanId,
//            };
//            investmentPlanRepo.Setup(r => r.GetInvestmentPlan(simulationId)).Returns(investmentPlanDto);
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork);

//            var libraries = await controller.GetInvestment(simulationId);

//            var returnedInvestmentDto = ActionResultAssertions.OkObject(libraries) as InvestmentDTO;
//            var expected = new InvestmentDTO
//            {
//                InvestmentPlan = investmentPlanDto,
//                ScenarioBudgets = budgets,
//            };
//            ObjectAssertions.Equivalent(expected, returnedInvestmentDto);
//        }


//        [Fact]
//        public async Task ScenarioPost_AdminUser_OkAndCallsRepositories()
//        {
//            var user = UserDtos.Admin();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var simulationRepo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
//            var simulationId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
//            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock(unitOfWork);
//            var investmentBudgetServiceMock = InvestmentBudgetServiceMocks.New();
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork, investmentBudgetServiceMock: investmentBudgetServiceMock);
//            var request = new InvestmentPagingSyncModel();
//            request.Investment = new InvestmentPlanDTO();

//            // Act
//            var result = await controller.UpsertInvestment(simulationId, request);

//            // Assert
//            ActionResultAssertions.Ok(result);
//            var budgetInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpsertOrDeleteScenarioBudgets));
//            var investmentPlanInvocation = investmentPlanRepo.SingleInvocationWithName(nameof(IInvestmentPlanRepository.UpsertInvestmentPlan));
//        }

//        [Fact]
//        public async Task ScenarioPost_UserHasModifyRights_OkAndCallsRepositories()
//        {
//            var user = UserDtos.Dbe();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var simulationRepo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
//            var simulationId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
//            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock(unitOfWork);
//            var investmentBudgetServiceMock = InvestmentBudgetServiceMocks.New();
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, investmentBudgetServiceMock: investmentBudgetServiceMock);
//            var request = new InvestmentPagingSyncModel();
//            request.Investment = new InvestmentPlanDTO();
//            var simulationUserDto = new SimulationUserDTO
//            {
//                UserId = user.Id,
//                Username = user.Username,
//                CanModify = true,
//            };
//            var simulationDto = new SimulationDTO
//            {
//                Id = simulationId,
//                Users = new List<SimulationUserDTO>
//                {
//                    simulationUserDto,
//                }
//            };
//            simulationRepo.Setup(sr => sr.GetSimulation(simulationId)).Returns(simulationDto);

//            // Act
//            var result = await controller.UpsertInvestment(simulationId, request);

//            // Assert
//            ActionResultAssertions.Ok(result);
//            var budgetInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpsertOrDeleteScenarioBudgets));
//            var investmentPlanInvocation = investmentPlanRepo.SingleInvocationWithName(nameof(IInvestmentPlanRepository.UpsertInvestmentPlan));
//        }

//        [Fact]
//        public async Task ScenarioPost_UserIsNotAuthorized_DoesNotCallRepositories()
//        {
//            var user1 = UserDtos.Dbe();
//            var user2 = UserDtos.Dbe();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
//            var simulationRepo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
//            var simulationId = Guid.NewGuid();
//            var hubService = HubServiceMocks.DefaultMock();
//            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
//            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock(unitOfWork);
//            var investmentBudgetServiceMock = InvestmentBudgetServiceMocks.New();
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService,  investmentBudgetServiceMock: investmentBudgetServiceMock);
//            var request = new InvestmentPagingSyncModel();
//            request.Investment = new InvestmentPlanDTO();
//            var simulationUserDto = new SimulationUserDTO
//            {
//                UserId = user2.Id,
//                Username = user1.Username,
//                CanModify = true,
//            };
//            var simulationDto = new SimulationDTO
//            {
//                Id = simulationId,
//                Users = new List<SimulationUserDTO>
//                {
//                    simulationUserDto,
//                }
//            };
//            simulationRepo.Setup(sr => sr.GetSimulation(simulationId)).Returns(simulationDto);

//            // Act
//            var result = await controller.UpsertInvestment(simulationId, request);

//            // Assert
//            ActionResultAssertions.Ok(result);
//            var budgetInvocations = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.UpsertOrDeleteScenarioBudgets));
//            var investmentPlanInvocations = investmentPlanRepo.InvocationsWithName(nameof(IInvestmentPlanRepository.UpsertInvestmentPlan));
//            Assert.Empty(budgetInvocations);
//            Assert.Empty(investmentPlanInvocations);
//            var message = hubService.SingleThreeArgumentUserMessage();
//            Assert.Contains(hubService.Object.errorList["Unauthorized"], message);
//        }

//        [Fact]
//        public async Task GetBudgetLibraries_AdminUser_CallsGetBudgetLibrariesNoChildrenOnRepo()
//        {
//            // Arrange
//            var user = UserDtos.Admin();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var budgetRepo = BudgetRepositoryMocks.New();
//            var budgetLibraryId = Guid.NewGuid();
//            var budgetLibraryDto = new BudgetLibraryDTO { Id = budgetLibraryId };
//            var budgetLibraryDtos = new List<BudgetLibraryDTO> { budgetLibraryDto };
//            budgetRepo.Setup(b => b.GetBudgetLibrariesNoChildren()).Returns(budgetLibraryDtos);
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork);
//            unitOfWork.SetupBudgetRepo(budgetRepo);

//            // Act
//            var result = await controller.GetBudgetLibraries();

//            // Assert
//            var getBudgetLibraryInvocations = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.GetBudgetLibrariesNoChildren));
//            Assert.Single(getBudgetLibraryInvocations);
//            var okObjResult = result as OkObjectResult;
//            Assert.NotNull(okObjResult.Value);
//            var dtos = (List<BudgetLibraryDTO>)Convert.ChangeType(okObjResult.Value,
//                typeof(List<BudgetLibraryDTO>));
//            var actualId = dtos.Single().Id;
//            Assert.Equal(budgetLibraryId, actualId);
//        }
//        [Fact]

//        public async Task GetBudgetLibraries_NonAdminUser_CallsGetBudgetLibrariesAccessibleToUserOnRepo()
//        {
//            // Arrange
//            var user = UserDtos.Dbe();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var budgetRepo = BudgetRepositoryMocks.New();
//            var budgetLibraryId = Guid.NewGuid();
//            var budgetLibraryDto = new BudgetLibraryDTO { Id = budgetLibraryId };
//            var budgetLibraryDtos = new List<BudgetLibraryDTO> { budgetLibraryDto };
//            budgetRepo.Setup(b => b.GetBudgetLibrariesNoChildrenAccessibleToUser(user.Id)).Returns(budgetLibraryDtos);
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork);
//            unitOfWork.SetupBudgetRepo(budgetRepo);

//            // Act
//            var result = await controller.GetBudgetLibraries();

//            // Assert
//            var getBudgetLibraryInvocations = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.GetBudgetLibrariesNoChildrenAccessibleToUser));
//            Assert.Single(getBudgetLibraryInvocations);
//            var okObjResult = result as OkObjectResult;
//            Assert.NotNull(okObjResult.Value);
//            var dtos = (List<BudgetLibraryDTO>)Convert.ChangeType(okObjResult.Value,
//                typeof(List<BudgetLibraryDTO>));
//            var actualId = dtos.Single().Id;
//            Assert.Equal(budgetLibraryId, actualId);
//        }

//        [Fact]
//        public async Task GetUsersOfLibrary_RequesterIsOwner_Gets()
//        {
//            var user1 = UserDtos.Dbe();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
//            var budgetRepo = BudgetRepositoryMocks.New();
//            var budgetLibraryId = Guid.NewGuid();
//            var ownerDto = new LibraryUserDTO
//            {
//                UserId = user1.Id,
//                AccessLevel = LibraryAccessLevel.Owner,
//            };
//            var userDtos = new List<LibraryUserDTO> { ownerDto };
//            budgetRepo.Setup(br => br.GetLibraryUsers(budgetLibraryId)).Returns(userDtos);
//            var budgetLibraryDto = new BudgetLibraryDTO { Id = budgetLibraryId };
//            var budgetLibraryDtos = new List<BudgetLibraryDTO> { budgetLibraryDto };
//            budgetRepo.SetupGetLibraryAccess(budgetLibraryId, user1.Id, LibraryAccessLevel.Owner);
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork);
//            unitOfWork.SetupBudgetRepo(budgetRepo);

//            var result = await controller.GetBudgetLibraryUsers(budgetLibraryId);

//            ActionResultAssertions.OkObject(result);
//            var value = (result as OkObjectResult).Value;
//            Assert.Equal(userDtos, value);
//        }

//        [Fact]
//        public async Task GetUsersOfLibrary_RequesterIsAdmin_Gets()
//        {
//            var user1 = UserDtos.Admin();
//            var user2 = UserDtos.Dbe();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
//            var budgetRepo = BudgetRepositoryMocks.New();
//            var budgetLibraryId = Guid.NewGuid();
//            var ownerDto = new LibraryUserDTO
//            {
//                UserId = user2.Id,
//                AccessLevel = LibraryAccessLevel.Owner,
//            };
//            var userDtos = new List<LibraryUserDTO> { ownerDto };
//            budgetRepo.Setup(br => br.GetLibraryUsers(budgetLibraryId)).Returns(userDtos);
//            var budgetLibraryDto = new BudgetLibraryDTO { Id = budgetLibraryId };
//            var budgetLibraryDtos = new List<BudgetLibraryDTO> { budgetLibraryDto };
//            budgetRepo.SetupGetLibraryAccess(budgetLibraryId, user1.Id, LibraryAccessLevel.Read);
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork);
//            unitOfWork.SetupBudgetRepo(budgetRepo);

//            var result = await controller.GetBudgetLibraryUsers(budgetLibraryId);

//            ActionResultAssertions.OkObject(result);
//            var value = (result as OkObjectResult).Value;
//            Assert.Equal(userDtos, value);
//        }


//        [Fact]
//        public async Task GetUsersOfLibrary_RequesterIsNeitherAdminNorOwner_DoesNotGet()
//        {
//            var user1 = UserDtos.Dbe();
//            var user2 = UserDtos.Dbe();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
//            var budgetRepo = BudgetRepositoryMocks.New();
//            var budgetLibraryId = Guid.NewGuid();
//            var ownerDto = new LibraryUserDTO
//            {
//                UserId = user2.Id,
//                AccessLevel = LibraryAccessLevel.Owner,
//            };
//            var userDtos = new List<LibraryUserDTO> { ownerDto };
//            budgetRepo.Setup(br => br.GetLibraryUsers(budgetLibraryId)).Returns(userDtos);
//            var budgetLibraryDto = new BudgetLibraryDTO { Id = budgetLibraryId };
//            var budgetLibraryDtos = new List<BudgetLibraryDTO> { budgetLibraryDto };
//            budgetRepo.SetupGetLibraryAccess(budgetLibraryId, user1.Id, LibraryAccessLevel.Read);
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);
//            unitOfWork.SetupBudgetRepo(budgetRepo);

//            var result = await controller.GetBudgetLibraryUsers(budgetLibraryId);

//            ActionResultAssertions.Ok(result);
//            var message = hubService.SingleThreeArgumentUserMessage();
//            Assert.Contains(ClaimHelper.LibraryUserListGetUnauthorizedMessage, message);
//        }

//        [Fact]
//        public async Task GetInvestment_RepositoriesReturnBudgetsAndInvestmentPlan_Expected()
//        {
//            var user1 = UserDtos.Dbe();
//            var user2 = UserDtos.Dbe();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
//            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
//            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock(unitOfWork);
//            var simulationId = Guid.NewGuid();
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);
//            var simulationRepo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
//            var simulationUser1 = new SimulationUserDTO
//            {
//                UserId = user1.Id,
//                Username = user1.Username,
//                CanModify = true,
//            };
//            var simulationDto = new SimulationDTO
//            {
//                Id = simulationId,
//                Users = new List<SimulationUserDTO> { simulationUser1 },
//            };
//            var budgetDtos = new List<BudgetDTO>();
//            var investmentPlanId = Guid.NewGuid();
//            var investmentPlanDto = new InvestmentPlanDTO
//            {
//                Id = investmentPlanId,
//            };
//            simulationRepo.Setup(s => s.GetSimulation(simulationId)).Returns(simulationDto);
//            budgetRepo.Setup(br => br.GetScenarioBudgets(simulationId)).Returns(budgetDtos);
//            investmentPlanRepo.Setup(ipr => ipr.GetInvestmentPlan(simulationId)).Returns(investmentPlanDto);
//            // Act
//            var result = await controller.GetInvestment(simulationId);

//            // Assert
//            var okObjResult = result as OkObjectResult;
//            var dto = (InvestmentDTO)Convert.ChangeType(okObjResult.Value, typeof(InvestmentDTO));
//            Assert.Equal(investmentPlanId, dto.InvestmentPlan.Id);
//            Assert.Equal(budgetDtos, dto.ScenarioBudgets);
//        }

//        /**************************INVESTMENT BUDGETS EXCEL FILE IMPORT/EXPORT TESTS***********************************/

//        [Fact]
//        public async Task RequestLibraryImport_InvalidMimeType_Throws()
//        {
//            var user = UserDtos.Admin();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork);

//            // Act + Asset
//            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
//                await controller.ImportLibraryInvestmentBudgetsExcelFile());
//            Assert.Equal("Request MIME type is invalid.", exception.Message);
//        }

//        [Fact]
//        public async Task ImportLibrary_FileNotFound_Throws()
//        {
//            // Arrange
//            var user = UserDtos.Admin();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var claims = SystemSecurityClaimLists.Admin();
//            var contextAccessor = HttpContextAccessorMocks.DefaultMock();
//            contextAccessor.AddClaims(claims);
//            var request = contextAccessor.Object.HttpContext.Request;
//            var formFileCollection = new FormFileCollection();
//            request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFileCollection);
//            var controller = TestInvestmentControllerSetup.CreateController(unitOfWork, contextAccessor.Object);
//            // Act + Asset

//            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
//                await controller.ImportLibraryInvestmentBudgetsExcelFile());

//            Assert.Equal("Investment budgets file not found.", exception.Message);
//        }

//        [Fact]
//        public async Task ImportLibraryInvestmentBudgetsExcelFile_BudgetLibraryIdNotFound_Throws()
//        {
//            // Arrange
//            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
//                "dummy.txt");
//            var accessor = CreateRequestForExceptionTesting(file);
//            var user = UserDtos.Admin();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var controller = TestInvestmentControllerSetup.CreateController(unitOfWork, accessor);

//            // Act + Asset
//            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
//                await controller.ImportLibraryInvestmentBudgetsExcelFile());
//            Assert.Equal("Request contained no budget library id.", exception.Message);
//        }

//        [Fact]
//        public async Task ShouldThrowConstraintWhenNoFilesForScenarioImport()
//        {
//            // Arrange
//            var accessor = CreateRequestForExceptionTesting();
//            var unitOfWork = UnitOfWorkMocks.New();
//            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
//            var controller = TestInvestmentControllerSetup.CreateController(unitOfWork, accessor);

//            // Act + Asset
//            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
//                await controller.ImportScenarioInvestmentBudgetsExcelFile());
//            Assert.Equal("Investment budgets file not found.", exception.Message);
//        }



//        [Fact]
//        public async Task ImportScenarioInvestmentBudgetsExcelFile_NoSimulationIdFound_Throws()
//        {
//            // Arrange
//            var user = UserDtos.Admin();
//            var unitOfWork = UnitOfWorkMocks.New();
//            var userRepo = UserRepositoryMocks.EveryoneExists(unitOfWork);
//            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
//                "dummy.txt");
//            var accessor = CreateRequestForExceptionTesting(file);
//            var controller = TestInvestmentControllerSetup.CreateController(unitOfWork, accessor);

//            // Act + Asset
//            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
//                await controller.ImportScenarioInvestmentBudgetsExcelFile());
//            Assert.Equal("Request contained no simulation id.", exception.Message);
//        }
        
//        [Fact]
//        public async Task ShouldThrowConstraintWhenNoMimeTypeForScenarioImport()
//        {
//            // Arrange
//            var user = UserDtos.Admin();
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user);
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork);

//            // Act + Asset
//            var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
//                await controller.ImportScenarioInvestmentBudgetsExcelFile());
//            Assert.Equal("Request MIME type is invalid.", exception.Message);
//        }
//    }
//}
