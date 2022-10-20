using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User
{
    public static class UserTestSetup
    {
        public static string NameForEntityInDb(IUnitOfWork unitOfWork, bool isAdmin = false)
        {
            var userName = RandomStrings.WithPrefix("user");
            unitOfWork.UserRepo.AddUser(userName, isAdmin);
            return userName;
        }

        public static async Task<UserDTO> ModelForEntityInDb(IUnitOfWork unitOfWork, bool isAdmin = false)
        {
            var userName = NameForEntityInDb(unitOfWork, isAdmin);
            var userDto = await unitOfWork.UserRepo.GetUserByUserName(userName);
            return userDto;
        }
    }
}
