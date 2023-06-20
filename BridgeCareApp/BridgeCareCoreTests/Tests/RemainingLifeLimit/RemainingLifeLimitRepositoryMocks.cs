using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class RemainingLifeLimitRepositoryMocks
    {
        public static Mock<IRemainingLifeLimitRepository> New(Mock<IUnitOfWork> mockUnitOfWork = null)
        {
            var repo = new Mock<IRemainingLifeLimitRepository>();
            if (mockUnitOfWork != null)
            {
                mockUnitOfWork.Setup(u => u.RemainingLifeLimitRepo).Returns(repo.Object);
            }
            return repo;
        }
        public static void SetupGetLibraryAccess(this Mock<IRemainingLifeLimitRepository> mock, Guid libraryId, LibraryUserAccessModel accessModel)
        {
            mock.Setup(r => r.GetLibraryAccess(libraryId, It.IsAny<Guid>())).Returns(accessModel);
        }
    }
}
