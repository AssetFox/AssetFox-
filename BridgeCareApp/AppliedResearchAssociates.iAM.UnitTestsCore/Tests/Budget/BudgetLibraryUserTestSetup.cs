using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetLibraryUserTestSetup
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

        /// <summary>The budgetLibrary and the user are both expected
        /// to exist before this method is called.</summary>  
        public static void SetUsersOfBudgetLibrary(
            IUnitOfWork unitOfWork,
            Guid budgetLibraryId,
            LibraryAccessLevel accessLevelForAllListedUsers,
            params Guid[] userIds)
        {
            var dtos = new List<LibraryUserDTO>();
            foreach (var userId in userIds) {
                var libraryUserDto = new LibraryUserDTO
                {
                    AccessLevel = accessLevelForAllListedUsers,
                    UserId = userId,
                };
                dtos.Add(libraryUserDto);
            }
            unitOfWork.BudgetRepo.UpsertOrDeleteUsers(budgetLibraryId, dtos);
        }
    }
}
