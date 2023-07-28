using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class TreatmentPerformanceFactorCloner
    {
        internal static TreatmentPerformanceFactorDTO Clone(TreatmentPerformanceFactorDTO treatmentPerformanceFactor)
        {

            var clone = new TreatmentPerformanceFactorDTO
            {
                Id = Guid.NewGuid(),
                Attribute = treatmentPerformanceFactor.Attribute,
                PerformanceFactor = treatmentPerformanceFactor.PerformanceFactor,
            };
            return clone;
        }

    }
}
