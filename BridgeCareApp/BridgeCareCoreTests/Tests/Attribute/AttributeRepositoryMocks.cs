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
    public static class AttributeRepositoryMocks
    {
        public static Mock<IAttributeRepository> New(Mock<IUnitOfWork> mockUnitOfWork = null)
        {
            var mock = new Mock<IAttributeRepository>();
            if (mockUnitOfWork != null )
            {
                mockUnitOfWork.Setup(m => m.AttributeRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
