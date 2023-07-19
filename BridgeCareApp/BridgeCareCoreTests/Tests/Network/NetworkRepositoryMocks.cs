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
    public static class NetworkRepositoryMocks
    {
        public static Mock<INetworkRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<INetworkRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.NetworkRepo).Returns(mock.Object);
            }
            return mock;
        }

        public static void SetupGetNetworkKeyAttribute(this Mock<INetworkRepository> mock, Guid networkId)
        {
            var networkKeyAttributeName = "NetworkKeyAttribute";
            mock.Setup(s => s.GetNetworkKeyAttribute(networkId)).Returns(networkKeyAttributeName);
        }
    }
}
