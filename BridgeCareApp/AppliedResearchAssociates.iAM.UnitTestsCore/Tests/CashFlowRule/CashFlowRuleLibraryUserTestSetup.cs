using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule
{
    public static class CashFlowRuleLibraryUserTestSetup
    {
        public static void SetUsersOfCashFlowRuleLibrary(
            IUnitOfWork unitOfWork,
            Guid cashFlowRuleLibraryId,
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
            unitOfWork.CashFlowRuleRepo.UpsertOrDeleteUsers(cashFlowRuleLibraryId, dtos);
        }
    }
}
