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
        public static Mock<IUnitOfWork> New()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            return unitOfWork;
        }
        public static Mock<IUnitOfWork> WithCurrentUser(UserDTO user)
        {
            var unitOfWork = New();
            unitOfWork.Setup(u => u.CurrentUser).Returns(user);
            var userRepository = UserRepositoryMocks.UserExists(user.Username);
            unitOfWork.Setup(u => u.UserRepo).Returns(userRepository.Object);
            return unitOfWork;
        }
    }
}
