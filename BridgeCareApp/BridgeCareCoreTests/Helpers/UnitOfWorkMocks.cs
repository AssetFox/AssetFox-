using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Moq;

namespace BridgeCareCoreTests.Helpers
{
    public static class UnitOfWorkMocks
    {
        public static Mock<IUnitOfWork> WithCurrentUser(UserDTO user)
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(u => u.CurrentUser).Returns(user);
            return unitOfWork;
        }
    }
}
