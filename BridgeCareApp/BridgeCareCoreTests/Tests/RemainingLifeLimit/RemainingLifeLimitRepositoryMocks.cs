using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
