using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Moq;

namespace BridgeCareCoreTests.Tests.Announcement
{
    public static class AnnouncementRepositoryMocks
    {
        public static Mock<IAnnouncementRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<IAnnouncementRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.AnnouncementRepo).Returns(mock.Object);
            }
            return mock;
        }
    }
}
