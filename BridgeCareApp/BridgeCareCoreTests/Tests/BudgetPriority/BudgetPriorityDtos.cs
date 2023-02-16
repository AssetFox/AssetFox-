using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCoreTests.Tests.BudgetPriority
{
    public static class BudgetPriorityDtos
    {
        public static BudgetPriorityDTO New(Guid? id = null, int priorityLevel = 0)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new BudgetPriorityDTO
            {
                Id = resolveId,
                BudgetPercentagePairs = new List<BudgetPercentagePairDTO>(),
                PriorityLevel = priorityLevel,
            };
            return dto;
        }
    }
}
