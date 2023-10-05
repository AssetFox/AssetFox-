using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Moq;

namespace BridgeCareCoreTests.Tests.Treatment
{
    public static class SelectableTreatmentRepositoryMocks
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
        public static void SetupGetLibraryAccess(this Mock<ISelectableTreatmentRepository> mock, Guid libraryId, LibraryUserAccessModel accessModel)
        {
            mock.Setup(r => r.GetLibraryAccess(libraryId, It.IsAny<Guid>())).Returns(accessModel);
        }
    }
}
