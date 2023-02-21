using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.RemainingLifeLimit
{
    public static class RemainingLifeLimitLibraryUserTestSetup
    {
        public static void SetUsersOfRemainingLifeLimitLibrary(
            IUnitOfWork unitOfWork,
            Guid remainingLifeLimitLibraryId,
            LibraryAccessLevel accessLevelForAllListedUsers,
            params Guid[] userIds)
        {
            var dtos = new List<LibraryUserDTO>();
            foreach (var userId in userIds)
            {
                var libraryUserDto = new LibraryUserDTO
                {
                    AccessLevel = accessLevelForAllListedUsers,
                    UserId = userId,
                };
                dtos.Add(libraryUserDto);
            }
            unitOfWork.RemainingLifeLimitRepo.UpsertOrDeleteUsers(remainingLifeLimitLibraryId, dtos);
        }
    }
}
