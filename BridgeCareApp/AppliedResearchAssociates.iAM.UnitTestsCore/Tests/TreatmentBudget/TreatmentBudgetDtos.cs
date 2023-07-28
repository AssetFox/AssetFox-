using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentBudgetDtos
    {
        public static TreatmentBudgetDTO Dto()
        {
            var testBudget = new TreatmentBudgetDTO
            {
                Id = Guid.NewGuid(),
                Name = "Budget Test 1"
            };
            return testBudget;
        }
    }
}
