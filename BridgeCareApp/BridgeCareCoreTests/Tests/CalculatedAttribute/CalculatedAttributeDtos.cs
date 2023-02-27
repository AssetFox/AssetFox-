using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;

namespace BridgeCareCoreTests.Tests
{
    public static class CalculatedAttributeDtos
    {
        public static CalculatedAttributeDTO Age(Guid? id = null, Guid? equationCriterionPairId = null, Guid? equationId = null)
        {
            var attribute = AttributeDtos.Age;
            var calculatedAttribute = ForAttribute(attribute, id, equationCriterionPairId, equationId);
            return calculatedAttribute;
        }

        public static CalculatedAttributeDTO EmptyForAttribute(AttributeDTO attribute, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new CalculatedAttributeDTO
            {
                Id = resolveId,
                Attribute = attribute.Name,
                CalculationTiming = 1,
            };
            return dto;
        }

        public static CalculatedAttributeDTO ForAttribute(AttributeDTO attribute, Guid? id = null, Guid? equationCriterionPairId = null, Guid? equationId = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var resolveEquationCriterionPairId = equationCriterionPairId ?? Guid.NewGuid(); ;
            var equation = new CalculatedAttributeEquationCriteriaPairDTO
            {
                Id = resolveEquationCriterionPairId,
                Equation = EquationDtos.AgePlus1(equationId),
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
