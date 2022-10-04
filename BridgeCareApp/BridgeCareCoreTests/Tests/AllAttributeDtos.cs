using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Models;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes
{
    public static class AllAttributeDtos
    {
        public static AllAttributeDTO BrKey(AllDataSource dataSource, Guid? id = null)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var dto = new AllAttributeDTO
            {
                AggregationRuleType = AggregationRuleTypeNames.Predominant,
                Command = "",
                DataSource = dataSource,
                DefaultValue = "Unknown",
                IsAscending = false,
                IsCalculated = false,
                Id = resolvedId,
                Maximum = null,
                Minimum = null,
                Name = "BRKEY",
                Type = AttributeTypeNames.String,
            };
            return dto;
        }
    }
}
