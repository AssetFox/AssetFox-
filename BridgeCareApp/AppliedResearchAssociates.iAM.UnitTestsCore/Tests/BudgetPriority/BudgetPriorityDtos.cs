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

        public static BudgetPriorityDTO WithPercentagePair(string budgetName, Guid budgetId, Guid? id = null, int priorityLevel = 0, int? year = null)
        {
            var dto = New(id, priorityLevel, year);
            var pair = new BudgetPercentagePairDTO
            {
                Id = Guid.NewGuid(),
                BudgetName = budgetName,
                BudgetId = budgetId,
                Percentage = 92,
            };
            dto.BudgetPercentagePairs.Add(pair);

            return dto;
        }
    }
}
