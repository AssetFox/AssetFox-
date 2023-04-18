using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetPriorityDtos
    {
        public static BudgetPriorityDTO New(Guid? id = null, int priorityLevel = 0, int? year = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new BudgetPriorityDTO
            {
                Id = resolveId,
                BudgetPercentagePairs = new List<BudgetPercentagePairDTO>(),
                PriorityLevel = priorityLevel,
                Year = year,
            };
            return dto;
        }
    }
}
