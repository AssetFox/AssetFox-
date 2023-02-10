using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Moq;

namespace BridgeCareCoreTests.Tests.AttributeDatum
{
    public static class AttributeDatumRepositoryMocks
    {
        public static Mock<IAttributeDatumRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<IAttributeDatumRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.AttributeDatumRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
