﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Utils;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.Investment;
using NuGet.ContentModel;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class InvestmentControllerBudgetLibraryUserUpdateTests
    {

        [Fact]
        public void ChangeUsersOfBudgetLibrary_RequesterIsNotOwner_Throws()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var user1 = UserDtos.Dbe(userId1);
            var user2 = UserDtos.Dbe(userId2);
            var user1Dto = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Owner,
                UserId = userId1,
            };
            var user2DtoRead = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = userId2,
            };
            var user2DtoModify = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Modify,
                UserId = userId2,
            };
            var currentUserList = new List<LibraryUserDTO> { user1Dto, user2DtoRead };
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibraryAccess(libraryId, user2.Id, LibraryAccessLevel.Read);
            budgetRepo.SetupGetLibaryUsers(libraryId, currentUserList);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user2);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var hubService = HubServiceMocks.DefaultMock();
            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);
            var updatedUserDtos = new List<LibraryUserDTO> { user1Dto, user2DtoModify };

            var exception = Assert.Throws<UnauthorizedAccessException>(() => controller.UpsertOrDeleteBudgetLibraryUsers(libraryId, updatedUserDtos));

            var message = exception.Message;
            Assert.Contains(ClaimHelper.LibraryAccessModificationUnauthorizedMessage, message);
        }

        [Fact]
        public void ChangeUsersOfBudgetLibrary_RequesterIsOwner_Does()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var user1 = UserDtos.Dbe(userId1);
            var user2 = UserDtos.Dbe(userId2);
            var user1Dto = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Owner,
                UserId = userId1,
            };
            var user2DtoRead = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = userId2,
            };
            var user2DtoModify = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Modify,
                UserId = userId2,
            };
            var currentUserList = new List<LibraryUserDTO> { user1Dto, user2DtoRead };
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibaryUsers(libraryId, currentUserList);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var hubService = HubServiceMocks.DefaultMock();
            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);
            var updatedUserDtos = new List<LibraryUserDTO> { user1Dto, user2DtoModify };

            controller.UpsertOrDeleteBudgetLibraryUsers(libraryId, updatedUserDtos);

            var invocations = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.UpsertOrDeleteUsers));
            var invocation = invocations.Single();
            var requestedUpdateLibraryId = invocation.Arguments[0];
            Assert.Equal(libraryId, requestedUpdateLibraryId);
            var requestedUserListUpdate = invocation.Arguments[1];
            Assert.Equal(updatedUserDtos, requestedUserListUpdate);
        }

        [Fact]
        public void ChangeUsersOfBudgetLibrary_RequesterAddsOwner_Throws()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var user1 = UserDtos.Dbe(userId1);
            var user2 = UserDtos.Dbe(userId2);
            var user1Dto = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Owner,
                UserId = userId1,
            };
            var user2DtoRead = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = userId2,
            };
            var user2DtoOwner = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Owner,
                UserId = userId2,
            };
            var currentUserList = new List<LibraryUserDTO> { user1Dto, user2DtoRead };
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibaryUsers(libraryId, currentUserList);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(user1);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var hubService = HubServiceMocks.DefaultMock();
            var controller = TestInvestmentControllerSetup.CreateNonAdminController(unitOfWork, hubService);
            var updatedUserDtos = new List<LibraryUserDTO> { user1Dto, user2DtoOwner };

            var exception = Assert.Throws<InvalidOperationException>(() => controller.UpsertOrDeleteBudgetLibraryUsers(libraryId, updatedUserDtos));

            var message = exception.Message;
            Assert.Contains(ClaimHelper.AddingOwnersIsNotAllowedMessage, message);
        }

        [Fact]
        public void ChangeUsersOfBudgetLibrary_RequesterIsAdmin_Does()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var user1 = UserDtos.Dbe(userId1);
            var user2 = UserDtos.Dbe(userId2);
            var adminUser = UserDtos.Admin;
            var user1Dto = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Owner,
                UserId = userId1,
            };
            var user2DtoRead = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = userId2,
            };
            var user2DtoModify = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Modify,
                UserId = userId2,
            };
            var currentUserList = new List<LibraryUserDTO> { user1Dto, user2DtoRead };
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibraryAccess(libraryId, user1.Id, LibraryAccessLevel.Owner);
            budgetRepo.SetupGetLibaryUsers(libraryId, currentUserList);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(adminUser);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var hubService = HubServiceMocks.DefaultMock();
            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork, hubService);
            var updatedUserDtos = new List<LibraryUserDTO> { user1Dto, user2DtoModify };

            controller.UpsertOrDeleteBudgetLibraryUsers(libraryId, updatedUserDtos);

            var invocations = budgetRepo.InvocationsWithName(nameof(IBudgetRepository.UpsertOrDeleteUsers));
            var invocation = invocations.Single();
            var requestedUpdateLibraryId = invocation.Arguments[0];
            Assert.Equal(libraryId, requestedUpdateLibraryId);
            var requestedUserListUpdate = invocation.Arguments[1];
            Assert.Equal(updatedUserDtos, requestedUserListUpdate);
        }


        [Fact]
        public void ChangeUsersOfBudgetLibrary_RequesterIsAdminButRequestRemovesOwner_Throws()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var user1 = UserDtos.Dbe(userId1);
            var user2 = UserDtos.Dbe(userId2);
            var adminUser = UserDtos.Admin;
            var user1Dto = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Owner,
                UserId = userId1,
            };
            var user1DtoModify = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Modify,
                UserId = userId1,
            };
            var user2DtoRead = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = userId2,
            };
            var currentUserList = new List<LibraryUserDTO> { user1Dto, user2DtoRead };
            var libraryId = Guid.NewGuid();
            var budgetRepo = BudgetRepositoryMocks.New();
            budgetRepo.SetupGetLibraryAccess(libraryId, user1.Id, LibraryAccessLevel.Owner);
            budgetRepo.SetupGetLibaryUsers(libraryId, currentUserList);
            var unitOfWork = UnitOfWorkMocks.WithCurrentUser(adminUser);
            unitOfWork.SetupBudgetRepo(budgetRepo);
            var hubService = HubServiceMocks.DefaultMock();
            var controller = TestInvestmentControllerSetup.CreateAdminController(unitOfWork, hubService);
            var updatedUserDtos = new List<LibraryUserDTO> { user1DtoModify, user2DtoRead };

            var exception = Assert.Throws<InvalidOperationException>(() => controller.UpsertOrDeleteBudgetLibraryUsers(libraryId, updatedUserDtos));

            var message = exception.Message;
            Assert.Contains(ClaimHelper.RemovingOwnersIsNotAllowedMessage, message);
        }
    }
}
