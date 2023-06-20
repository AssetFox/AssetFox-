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
    public static class TreatmentLibraryUserMocks
    {
        public static Mock<ITreatmentLibraryUserRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<ITreatmentLibraryUserRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.TreatmentLibraryUserRepo).Returns(mock.Object);
            }
            return mock;
        }
        public static void SetupGetLibraryAccess(this Mock<ITreatmentLibraryUserRepository> mock, Guid libraryId, LibraryUserAccessModel accessModel)
        {
            mock.Setup(r => r.GetLibraryAccess(libraryId, It.IsAny<Guid>())).Returns(accessModel);
        }
    }
}
