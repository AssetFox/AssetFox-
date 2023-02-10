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
        public static BudgetPriorityDTO New()
        {
            var dto = new BudgetPriorityDTO
            {
                Id = Guid.NewGuid(),
            };
            return dto;
        }
    }
}
