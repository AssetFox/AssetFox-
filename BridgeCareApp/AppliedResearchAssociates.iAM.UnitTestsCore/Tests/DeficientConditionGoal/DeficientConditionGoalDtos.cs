using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    internal static class DeficientConditionGoalDtos
    {
        public static DeficientConditionGoalDTO CulvDurationN()
        {
            var dto = new DeficientConditionGoalDTO
            {
                AllowedDeficientPercentage= 100,
                Attribute = TestAttributeNames.CulvDurationN,
                DeficientLimit = 1,
                Id = Guid.NewGuid(),
                Name = "Test Name",
            };
            return dto;
        }

    }
}
