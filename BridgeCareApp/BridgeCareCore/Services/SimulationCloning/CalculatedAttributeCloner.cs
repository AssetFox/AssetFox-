using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace BridgeCareCore.Services.SimulationCloning
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

    }
}
