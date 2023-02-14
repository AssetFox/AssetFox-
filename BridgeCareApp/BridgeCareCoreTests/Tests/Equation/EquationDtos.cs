using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCoreTests.Tests
{
    public static class EquationDtos
    {
        public static EquationDTO AgePlus1(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var equation = new EquationDTO
            {
                Id = resolveId,
                Expression = "[AGE] + 1"
            };
            return equation;
        }
    }
}
