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
    public static class MaintainableAssetRepositoryMocks
    {
        public static Mock<IMaintainableAssetRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<IMaintainableAssetRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.MaintainableAssetRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
