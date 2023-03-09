using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.PerformanceCurve
{
    public static class PerformanceCurveLibraryUserTestSetup
    {
        public static void SetUsersOfPerformanceCurveLibrary(
            IUnitOfWork unitOfWork,
            Guid performanceCurveLibraryId,
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
                    UserName = "Test User"
                };
                dtos.Add(libraryUserDto);
            }
            unitOfWork.PerformanceCurveRepo.UpsertOrDeleteUsers(performanceCurveLibraryId, dtos);
        }
    }
}
