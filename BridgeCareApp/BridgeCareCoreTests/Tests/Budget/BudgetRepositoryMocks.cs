using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class BudgetRepositoryMocks
    {
        public static Mock<IBudgetRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<IBudgetRepository>();
            unitOfWork?.Setup(u => u.BudgetRepo).Returns(mock.Object);
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
            LibraryUserDTO access = null;
            if (accessLevel != null)
            {
                access = new LibraryUserDTO
                {
                    AccessLevel = accessLevel.Value,
                    UserId = userId,
                };
            }
            var dto = new LibraryUserAccessModel
            {
                LibraryExists = true,
                UserId = userId,
                Access = access,
            };
            mock.Setup(m => m.GetLibraryAccess(libraryId, userId)).Returns(dto);
        }

        public static void SetupGetLibaryUsers(this Mock<IBudgetRepository> repository, Guid libraryId, List<LibraryUserDTO> users)
        {
            repository.Setup(r => r.GetLibraryUsers(libraryId)).Returns(users);
        }

        public static List<BudgetLibraryDTO> GetUpsertBudgetLibraryCalls(this Mock<IBudgetRepository> mock)
        {
            var r = new List<BudgetLibraryDTO>();
            var invocations = mock.Invocations.Where(i => i.Method.Name == nameof(IBudgetRepository.UpsertBudgetLibrary)).ToList();
            foreach (var invocation in invocations)
            {
                var dto = (BudgetLibraryDTO)invocation.Arguments[0];
                r.Add(dto);
            }
            return r;
        }
    }
}
