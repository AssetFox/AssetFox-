using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class BudgetRepositoryMocks
    {
        public static Mock<IBudgetRepository> New()
        {
            var mock = new Mock<IBudgetRepository>();
            return mock;
        }

        public static void SetupLibraryAccessLibraryDoesNotExist(this Mock<IBudgetRepository> mock, Guid libraryId)
        {
            var libraryAccess = LibraryAccessModels.LibraryDoesNotExist;
            mock.Setup(m => m.GetLibraryAccess(libraryId, It.IsAny<Guid>())).Returns(libraryAccess);
        }

        ///<summary>Pass in null for the access level to tell the mock to return a LibraryAccessModel with no users.</summary>  
        public static void SetupGetLibraryAccess(this Mock<IBudgetRepository> mock, Guid libraryId, Guid userId, LibraryAccessLevel? accessLevel)
        {
            var users = new List<LibraryUserDTO>();
            if (accessLevel != null)
            {
                var user = new LibraryUserDTO
                {
                    AccessLevel = accessLevel.Value,
                    UserId = userId,
                };
                users.Add(user);
            }
            var dto = new LibraryAccessModel
            {
                LibraryExists = true,
                UserId = userId,
                Users = users,
            };
            mock.Setup(m => m.GetLibraryAccess(libraryId, userId)).Returns(dto);
        }

        public static List<(BudgetLibraryDTO, bool)> GetUpsertBudgetLibraryCalls(this Mock<IBudgetRepository> mock)
        {
            var r = new List<(BudgetLibraryDTO, bool)>();
            var invocations = mock.Invocations.Where(i => i.Method.Name == nameof(IBudgetRepository.UpsertBudgetLibrary)).ToList();
            foreach (var invocation in invocations)
            {
                var dto = (BudgetLibraryDTO)invocation.Arguments[0];
                var accessModificationAllowed = (bool)invocation.Arguments[1];
                r.Add((dto, accessModificationAllowed));
            }
            return r;
        }
    }
}
