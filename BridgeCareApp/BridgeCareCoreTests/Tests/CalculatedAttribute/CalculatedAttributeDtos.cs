using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCoreTests.Tests
{
    public static class CalculatedAttributeDtos
    {
        public static CalculatedAttributeDTO ForAttribute(AttributeDTO attribute, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var equation = new CalculatedAttributeEquationCriteriaPairDTO
            {
                Id = Guid.NewGuid(),
                Equation = EquationDtos.AgePlus1(),
            };
            var equations = new List<CalculatedAttributeEquationCriteriaPairDTO> { equation };
            var dto = new CalculatedAttributeDTO
            {
                Id = resolveId,
                Attribute = attribute.Name,
                CalculationTiming = 1,
                Equations = equations,
            };
            return dto;
        }
    }
}
