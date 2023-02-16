using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCoreTests.Tests.BudgetPriority
{
    public static class SimpleBudgetDetailDtos
    {
        public static SimpleBudgetDetailDTO Dto(Guid? id = null, string name = "Budget")
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new SimpleBudgetDetailDTO
            {
                Id = resolveId,
                Name = name
            };
            return dto;
        }
    }
}
