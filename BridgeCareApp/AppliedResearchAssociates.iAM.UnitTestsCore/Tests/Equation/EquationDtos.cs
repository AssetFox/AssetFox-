using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
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

        public static EquationDTO WithExpression(Guid id, string expression)
        {
            var equation = new EquationDTO
            {
                Id = id,
                Expression = expression,
            };
            return equation;
        }
    }
}
