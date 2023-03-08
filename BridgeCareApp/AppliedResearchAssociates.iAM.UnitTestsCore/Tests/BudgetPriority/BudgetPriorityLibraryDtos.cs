using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class BudgetPriorityLibraryDtos
    {

        public const string BudgetPriorityLibraryName = "BudgetPriorityLibrary";

        public static BudgetPriorityLibraryDTO New(Guid? id = null)
        {
            var dto = new BudgetPriorityLibraryDTO
            {
                Id = id ?? Guid.NewGuid(),
                Name = BudgetPriorityLibraryName,
            };
            return dto;
        }
    }
}
