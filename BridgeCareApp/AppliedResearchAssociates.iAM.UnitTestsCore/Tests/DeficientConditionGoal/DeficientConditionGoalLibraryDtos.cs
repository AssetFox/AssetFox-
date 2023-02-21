using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DeficientConditionGoal
{
    public static class DeficientConditionGoalLibraryDtos
    {
        public static DeficientConditionGoalLibraryDTO Empty()
        {
            var dto = new DeficientConditionGoalLibraryDTO
            {
                Id = Guid.NewGuid(),
            };
            return dto;
        }
    }
}
