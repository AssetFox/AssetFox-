using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class CalculatedAttributeCloner
    {
        internal static CalculatedAttributeDTO Clone(CalculatedAttributeDTO calculatedAttribute)
        {
            var clone = new CalculatedAttributeDTO
            {
                Id = Guid.NewGuid(),
                LibraryId = calculatedAttribute.LibraryId,
                Attribute = calculatedAttribute.Attribute,
                CalculationTiming = calculatedAttribute.CalculationTiming,
                Equations = calculatedAttribute.Equations,
                IsModified = calculatedAttribute.IsModified,
            };
            return clone;
        }
        internal static List<CalculatedAttributeDTO> CloneList(IEnumerable<CalculatedAttributeDTO> calculatedAttributes)
        {
            var clone = new List<CalculatedAttributeDTO>();
            foreach (var calculatedAttribute in calculatedAttributes)
            {
                var childClone = Clone(calculatedAttribute);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
