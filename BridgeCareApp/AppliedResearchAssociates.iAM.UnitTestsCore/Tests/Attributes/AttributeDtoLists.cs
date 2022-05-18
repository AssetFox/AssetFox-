using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes
{
    public static class AttributeDtoLists
    {
        public static List<AttributeDTO> AttributeSetupDtos()
            => new List<AttributeDTO>
            {
                AttributeDtos.ActionType,
                AttributeDtos.AdtTotal,
                AttributeDtos.DeckSeeded,
                AttributeDtos.SubSeeded,
                AttributeDtos.SupSeeded,
            };
    }
}
