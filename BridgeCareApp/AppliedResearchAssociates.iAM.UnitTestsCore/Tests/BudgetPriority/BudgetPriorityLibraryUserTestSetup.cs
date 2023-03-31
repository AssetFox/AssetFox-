using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.BudgetPriority
{
    public static class BudgetPriorityLibraryUserTestSetup
    {
        public static void SetUsersOfBudgetPriorityLibrary(
            IUnitOfWork unitOfWork,
            Guid budgetPriorityLibraryId,
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
            unitOfWork.BudgetPriorityRepo.UpsertOrDeleteUsers(budgetPriorityLibraryId, dtos);
        }
    }
}
