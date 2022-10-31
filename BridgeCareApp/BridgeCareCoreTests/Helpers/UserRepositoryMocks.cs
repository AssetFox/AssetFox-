using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Moq;

namespace BridgeCareCoreTests.Helpers
{
    public static class UserRepositoryMocks
    {
        public static Mock<IUserRepository> EveryoneExists(Mock<IUnitOfWork> mockUnitOfWork = null)
        {
            var mockUserRepo = new Mock<IUserRepository>();
            mockUnitOfWork?.Setup(uow => uow.UserRepo).Returns(mockUserRepo.Object);
            mockUserRepo.Setup(_ => _.UserExists(It.IsAny<string>())).Returns(true);
            return mockUserRepo;
        }

        public static Mock<IUserRepository> UserExists(string name)
        {
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(_ => _.UserExists(name)).Returns(true);
            return mockUserRepo;
        }
    }
}
