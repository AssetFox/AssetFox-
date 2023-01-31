using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AbbreviatedAttributeDtos
    {
        public static AbbreviatedAttributeDTO CulvDurationN
        => new()
        {
            Name = TestAttributeNames.CulvDurationN,
            Type = AttributeTypeNames.Number,
        };

        public static AbbreviatedAttributeDTO ActionType
            => new()
            {
                Name = TestAttributeNames.ActionType,
                Type = AttributeTypeNames.String,
            };
    }
}
