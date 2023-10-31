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
        public static TreatmentBudgetDTO Dto(string name = "Budget Test 1")
        {
            var testBudget = new TreatmentBudgetDTO
            {
                Id = Guid.NewGuid(),
                Name = name,
            };
            return testBudget;
        }
    }
}
