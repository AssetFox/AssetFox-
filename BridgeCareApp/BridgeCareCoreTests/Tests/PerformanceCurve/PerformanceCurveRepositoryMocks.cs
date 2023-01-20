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
    public static class PerformanceCurveRepositoryMocks
    {
        public static Mock<IPerformanceCurveRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<IPerformanceCurveRepository>();
            if (unitOfWork!=null)
            {
                unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
