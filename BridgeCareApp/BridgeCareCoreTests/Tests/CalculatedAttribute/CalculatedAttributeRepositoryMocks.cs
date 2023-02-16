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
    public static class CalculatedAttributeRepositoryMocks
    {
        public static Mock<ICalculatedAttributesRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<ICalculatedAttributesRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.CalculatedAttributeRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
