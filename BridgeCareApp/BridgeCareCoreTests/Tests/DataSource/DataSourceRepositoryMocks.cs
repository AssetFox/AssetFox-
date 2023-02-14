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
    public static class DataSourceRepositoryMocks
    {
        public static Mock<IDataSourceRepository> New(Mock<IUnitOfWork> mockUnitOfWork = null)
        {
            var mock = new Mock<IDataSourceRepository>();
            if (mockUnitOfWork != null)
            {
                mockUnitOfWork.Setup(u => u.DataSourceRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
