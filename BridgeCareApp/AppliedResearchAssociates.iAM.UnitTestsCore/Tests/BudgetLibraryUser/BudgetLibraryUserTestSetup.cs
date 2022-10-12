using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetLibraryUserTestSetup
    {
        public static async Task<List<LibraryUserDTO>> GetUserList(IUnitOfWork unitOfWork)
        {
            var returnList = new List<LibraryUserDTO>();
            var userDto = await UserTestSetup.ModelForEntityInDb(unitOfWork, false);

            var user1 = Dto(userDto.Id, LibraryAccessLevel.Owner);
            returnList.Add(user1);

            return returnList;
        }

        public static LibraryUserDTO Dto(Guid userId, LibraryAccessLevel accessLevel)
        {
            var dto = new LibraryUserDTO
            {
                UserId = userId,
                AccessLevel = accessLevel,
            };

            return dto;
        }

        public static LibraryUserDTO Dto()
        {
            var dto = new LibraryUserDTO
            {
                UserId = Guid.NewGuid(),
                AccessLevel = LibraryAccessLevel.Owner,
            };

            return dto;
        }
    }
}
