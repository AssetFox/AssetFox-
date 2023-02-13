using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetLibraryDtos
    {
        public static BudgetLibraryDTO New(Guid? id = null, string name = "BudgetLibrary")
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new BudgetLibraryDTO
            {
                Id = resolveId,
                Name = name,
                Budgets = new List<BudgetDTO>(),
            };
            return dto;
        }
    }
}
