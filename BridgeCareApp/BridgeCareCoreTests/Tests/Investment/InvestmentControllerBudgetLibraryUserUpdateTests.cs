//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
//using AppliedResearchAssociates.iAM.DTOs;
//using AppliedResearchAssociates.iAM.DTOs.Enums;
//using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
//using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
//using BridgeCareCore.Utils;
//using BridgeCareCoreTests.Helpers;
//using NuGet.ContentModel;
//using Xunit;

//namespace BridgeCareCoreTests.Tests
//{
//    public class InvestmentControllerBudgetLibraryUserUpdateTests
//    {

//        [Fact]
//        public async Task ChangeUsersOfBudgetLibrary_RequesterIsNotOwner_OkButUnauthorizedHubMessage()
//        {
//            var userId1 = Guid.NewGuid();
//            var userId2 = Guid.NewGuid();
//            var user1 = UserDtos.Dbe(userId1);
//            var user2 = UserDtos.Dbe(userId2);
//            var user1Dto = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Owner,
//                UserId = userId1,
//            };
//            var user2DtoRead = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Read,
//                UserId = userId2,
//            };
//            var user2DtoModify = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Modify,
//                UserId = userId2,
//            };
//            var currentUserList = new List<LibraryUserDTO> { user1Dto, user2DtoRead };
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibraryAccess(libraryId, user2.Id, LibraryAccessLevel.Read);
//            budgetRepo.SetupGetLibaryUsers(libraryId, currentUserList);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user2);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);
//            var updatedUserDtos = new List<LibraryUserDTO> { user1Dto, user2DtoModify };

//            var result = await controller.UpsertOrDeleteBudgetLibraryUsers(libraryId, updatedUserDtos);

//            ActionResultAssertions.Ok(result);
//            var hubMessage = hubService.SingleThreeArgumentUserMessage();
//            Assert.Contains(ClaimHelper.LibraryAccessModificationUnauthorizedMessage, hubMessage);
//        }

//        [Fact]
//        public async Task ChangeUsersOfBudgetLibrary_RequesterIsOwner_Does()
//        {
//            var userId1 = Guid.NewGuid();
//            var userId2 = Guid.NewGuid();
//            var user1 = UserDtos.Dbe(userId1);
//            var user2 = UserDtos.Dbe(userId2);
//            var user1Dto = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Owner,
//                UserId = userId1,
//            };
//            var user2DtoRead = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Read,
//                UserId = userId2,
//            };
//            var user2DtoModify = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Modify,
//                UserId = userId2,
//            };
//            var currentUserList = new List<LibraryUserDTO> { user1Dto, user2DtoRead };
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibaryUsers(libraryId, currentUserList);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);
//            var updatedUserDtos = new List<LibraryUserDTO> { user1Dto, user2DtoModify };

//            var result = await controller.UpsertOrDeleteBudgetLibraryUsers(libraryId, updatedUserDtos);

//            ActionResultAssertions.Ok(result);
//            var invocations = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.UpsertOrDeleteUsers));
//            var invocation = invocations.Single();
//            var requestedUpdateLibraryId = invocation.Arguments[0];
//            Assert.Equal(libraryId, requestedUpdateLibraryId);
//            var requestedUserListUpdate = invocation.Arguments[1];
//            Assert.Equal(updatedUserDtos, requestedUserListUpdate);
//        }

//        [Fact]
//        public async Task ChangeUsersOfBudgetLibrary_RequestAddsOwner_BadRequest()
//        {
//            var userId1 = Guid.NewGuid();
//            var userId2 = Guid.NewGuid();
//            var user1 = UserDtos.Dbe(userId1);
//            var user2 = UserDtos.Dbe(userId2);
//            var user1Dto = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Owner,
//                UserId = userId1,
//            };
//            var user2DtoRead = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Read,
//                UserId = userId2,
//            };
//            var user2DtoOwner = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Owner,
//                UserId = userId2,
//            };
//            var currentUserList = new List<LibraryUserDTO> { user1Dto, user2DtoRead };
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibaryUsers(libraryId, currentUserList);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);
//            var updatedUserDtos = new List<LibraryUserDTO> { user1Dto, user2DtoOwner };

//            var result = await controller.UpsertOrDeleteBudgetLibraryUsers(libraryId, updatedUserDtos);

//            ActionResultAssertions.BadRequest(result);
//            var invocations = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.UpsertOrDeleteUsers));
//            Assert.Empty(invocations);
//            var hubMessage = hubService.SingleThreeArgumentUserMessage();
//            Assert.Contains(ClaimHelper.AddingOwnersIsNotAllowedMessage, hubMessage);
//        }

//        [Fact]
//        public async Task ChangeUsersOfBudgetLibrary_RequesterIsAdmin_Does()
//        {
//            var userId1 = Guid.NewGuid();
//            var userId2 = Guid.NewGuid();
//            var user1 = UserDtos.Dbe(userId1);
//            var user2 = UserDtos.Dbe(userId2);
//            var adminUser = UserDtos.Admin();
//            var user1Dto = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Owner,
//                UserId = userId1,
//            };
//            var user2DtoRead = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Read,
//                UserId = userId2,
//            };
//            var user2DtoModify = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Modify,
//                UserId = userId2,
//            };
//            var currentUserList = new List<LibraryUserDTO> { user1Dto, user2DtoRead };
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibraryAccess(libraryId, user1.Id, LibraryAccessLevel.Owner);
//            budgetRepo.SetupGetLibaryUsers(libraryId, currentUserList);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(adminUser);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork, hubService);
//            var updatedUserDtos = new List<LibraryUserDTO> { user1Dto, user2DtoModify };

//            var result = await controller.UpsertOrDeleteBudgetLibraryUsers(libraryId, updatedUserDtos);

//            ActionResultAssertions.Ok(result);
//            var invocations = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.UpsertOrDeleteUsers));
//            var invocation = invocations.Single();
//            var requestedUpdateLibraryId = invocation.Arguments[0];
//            Assert.Equal(libraryId, requestedUpdateLibraryId);
//            var requestedUserListUpdate = invocation.Arguments[1];
//            Assert.Equal(updatedUserDtos, requestedUserListUpdate);
//        }


//        [Fact]
//        public async Task ChangeUsersOfBudgetLibrary_RequesterIsAdminButRequestRemovesOwner_BadRequest()
//        {
//            var userId1 = Guid.NewGuid();
//            var userId2 = Guid.NewGuid();
//            var user1 = UserDtos.Dbe(userId1);
//            var user2 = UserDtos.Dbe(userId2);
//            var adminUser = UserDtos.Admin();
//            var user1Dto = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Owner,
//                UserId = userId1,
//            };
//            var user1DtoModify = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Modify,
//                UserId = userId1,
//            };
//            var user2DtoRead = new LibraryUserDTO
//            {
//                AccessLevel = LibraryAccessLevel.Read,
//                UserId = userId2,
//            };
//            var currentUserList = new List<LibraryUserDTO> { user1Dto, user2DtoRead };
//            var libraryId = Guid.NewGuid();
//            var budgetRepo = BudgetRepositoryMocks.New();
//            budgetRepo.SetupGetLibraryAccess(libraryId, user1.Id, LibraryAccessLevel.Owner);
//            budgetRepo.SetupGetLibaryUsers(libraryId, currentUserList);
//            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(adminUser);
//            unitOfWork.SetupBudgetRepo(budgetRepo);
//            var hubService = HubServiceMocks.DefaultMock();
//            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork, hubService);
//            var updatedUserDtos = new List<LibraryUserDTO> { user1DtoModify, user2DtoRead };

//            var result = await controller.UpsertOrDeleteBudgetLibraryUsers(libraryId, updatedUserDtos);

//            ActionResultAssertions.BadRequest(result);
//            var invocations = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.UpsertOrDeleteUsers));
//            Assert.Empty(invocations);
//            var hubMessage = hubService.SingleThreeArgumentUserMessage();
//            Assert.Contains(ClaimHelper.RemovingOwnersIsNotAllowedMessage, hubMessage);
//        }
//    }
//}
