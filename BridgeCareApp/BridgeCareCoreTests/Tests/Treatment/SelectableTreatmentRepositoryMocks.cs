using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Moq;

namespace BridgeCareCoreTests.Tests.Treatment
{
    public class SelectableTreatmentRepositoryMocks
    {
        public static Mock<ISelectableTreatmentRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<ISelectableTreatmentRepository>();
            if (unitOfWork!=null)
            {
                unitOfWork.Setup(u => u.SelectableTreatmentRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
