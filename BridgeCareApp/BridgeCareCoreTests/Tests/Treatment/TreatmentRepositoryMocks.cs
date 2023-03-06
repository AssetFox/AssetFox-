using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;
using Moq;

namespace BridgeCareCoreTests.Tests.Treatment
{
    public static class TreatmentRepositoryMocks
    {
        public static Mock<ITreatmentLibraryUserRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<ITreatmentLibraryUserRepository>();
            unitOfWork?.Setup(u => u.TreatmentLibraryUserRepo).Returns(mock.Object);
            return mock;
        }

        public static void SetupLibraryAccessLibraryDoesNotExist(this Mock<ITreatmentLibraryUserRepository> mock, Guid libraryId)
        {
            var libraryAccess = LibraryAccessModels.LibraryDoesNotExist;
            mock.Setup(m => m.GetLibraryAccess(libraryId, It.IsAny<Guid>())).Returns(libraryAccess);
        }

        ///<summary>Pass in null for the access level to tell the mock to return a LibraryAccessModel with no users.</summary>  
        public static void SetupGetLibraryAccess(this Mock<ITreatmentLibraryUserRepository> mock, Guid libraryId, Guid userId, LibraryAccessLevel? accessLevel)
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

        public static void SetupGetLibaryUsers(this Mock<ITreatmentLibraryUserRepository> repository, Guid libraryId, List<LibraryUserDTO> users)
        {
            repository.Setup(r => r.GetLibraryUsers(libraryId)).Returns(users);
        }

        public static List<TreatmentLibraryDTO> GetUpsertTreatmentLibraryCalls(this Mock<ITreatmentLibraryUserRepository> mock)
        {
            var r = new List<TreatmentLibraryDTO>();
            var invocations = mock.Invocations.Where(i => i.Method.Name == nameof(ITreatmentLibraryUserRepository.UpsertTreatmentLibraryUser)).ToList();
            foreach (var invocation in invocations)
            {
                var dto = (TreatmentLibraryDTO)invocation.Arguments[0];
                r.Add(dto);
            }
            return r;
        }
    }
}
