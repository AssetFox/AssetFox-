using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCoreTests.Tests.BudgetPriority
{
    internal class BudgetPriorityLibraryDtos
    {

        public const string BudgetPriorityLibraryEntityName = "BudgetPriorityLibraryEntity";

        public static BudgetPriorityLibraryDTO New(Guid? id = null)
        {
            var dto = new BudgetPriorityLibraryDTO
            {
                Id = id ?? Guid.NewGuid(),
                Name = BudgetPriorityLibraryEntityName,
            };
            return dto;
        }
    }
}
