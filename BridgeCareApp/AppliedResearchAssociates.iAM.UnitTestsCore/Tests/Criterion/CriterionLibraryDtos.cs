using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class CriterionLibraryDtos
    {
        public static CriterionLibraryDTO Dto(Guid? id = null, string mergedCriteriaExpression = "mergedCriteriaExpression")
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new CriterionLibraryDTO
            {
                Id = resolveId,
                MergedCriteriaExpression = mergedCriteriaExpression,
                IsSingleUse = true,
            };
            return dto;
        }
    }
}
