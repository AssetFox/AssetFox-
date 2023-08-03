using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TargetConditionGoalDtos
    {
        public static TargetConditionGoalDTO Dto(string attribute,
            Guid? id = null,
            string name = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var resolveName = name ?? RandomStrings.Length11();
            var dto = new TargetConditionGoalDTO
            {
                Id = resolveId,
                Name = resolveName,
                Attribute = attribute,
                Target = 1,
            };
            return dto;
        }
        public static TargetConditionGoalDTO DtoWithCriterionLibrary(string attribute,
            Guid? id = null,
            string name = null)
        {
            var criterionLibrary = CriterionLibraryDtos.Dto();
            var resolveId = id ?? Guid.NewGuid();
            var resolveName = name ?? RandomStrings.Length11();
            var dto = new TargetConditionGoalDTO
            {
                Id = resolveId,
                Name = resolveName,
                Attribute = attribute,
                Target = 1,
                CriterionLibrary = criterionLibrary
            };
            return dto;
        }
    }
}
