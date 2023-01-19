using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class DeficientConditionGoalDtos
    {
        public static DeficientConditionGoalDTO CulvDurationN(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new DeficientConditionGoalDTO
            {
                AllowedDeficientPercentage= 100,
                Attribute = TestAttributeNames.CulvDurationN,
                DeficientLimit = 1,
                Id = resolveId,
                Name = "Test Name",
            };
            return dto;
        }

    }
}
