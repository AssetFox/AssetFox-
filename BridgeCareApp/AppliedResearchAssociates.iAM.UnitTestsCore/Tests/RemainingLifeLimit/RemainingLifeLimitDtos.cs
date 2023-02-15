using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.RemainingLifeLimit
{
    public static class RemainingLifeLimitDtos
    {
        public static RemainingLifeLimitDTO Dto(string attribute, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new RemainingLifeLimitDTO
            {
                Attribute = attribute,
                Id = resolveId,
            };
            return dto;
        }
    }
}
