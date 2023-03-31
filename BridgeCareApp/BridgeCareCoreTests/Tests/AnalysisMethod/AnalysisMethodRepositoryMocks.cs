using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Moq;

namespace BridgeCareCoreTests.Tests.AnalysisMethod
{
    internal class AnalysisMethodRepositoryMocks
    {
        public static Mock<IAnalysisMethodRepository> DefaultMock(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<IAnalysisMethodRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.AnalysisMethodRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
