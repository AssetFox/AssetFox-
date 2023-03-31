using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DeficientConditionGoal
{
    public static class DeficientConditionGoalLibraryUserTestSetup
    {
        public static LibraryUserDTO CreateLibraryUserDto(Guid userId, LibraryAccessLevel accessLevel = LibraryAccessLevel.Read)
        {
            var dto = new LibraryUserDTO
            {
                UserId = userId,
                AccessLevel = accessLevel,
            };
            return dto;
        }

        public static void SetUsersOfDeficientConditionGoalLibrary(
            IUnitOfWork unitOfWork,
            Guid deficientConditionGoalLibraryId,
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
            unitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteUsers(deficientConditionGoalLibraryId, dtos);
        }
    }
}
