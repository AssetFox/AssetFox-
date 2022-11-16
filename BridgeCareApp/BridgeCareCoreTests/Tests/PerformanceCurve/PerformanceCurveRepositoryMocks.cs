using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;
using Moq;

namespace BridgeCareCoreTests.Tests.PerformanceCurve
{
    public static class PerformanceCurveRepositoryMocks
    {
        public static Mock<IPerformanceCurveRepository> New()
        {
            var mock = new Mock<IPerformanceCurveRepository>();
            return mock;
        }

        ///<summary>Pass in null for the access level to tell the mock to return a LibraryAccessModel with no users.</summary>  
        public static void SetupGetLibraryAccess(this Mock<IPerformanceCurveRepository> mock, Guid libraryId, Guid userId, LibraryAccessLevel? accessLevel)
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

    }
}
