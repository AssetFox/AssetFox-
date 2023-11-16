using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Benefit
{
    public static class BenefitDtos
    {
        public static BenefitDTO Dto(string attribute, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new BenefitDTO
            {
                Attribute = attribute,
                Id = resolveId,
                Limit = 100,
            };
            return dto;
        }
    }
}
