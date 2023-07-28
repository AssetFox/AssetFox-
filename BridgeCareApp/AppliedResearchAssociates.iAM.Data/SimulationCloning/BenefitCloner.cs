using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    
    internal class BenefitCloner
    {
        internal static BenefitDTO Clone(BenefitDTO benefit)
        {
            var clone = new BenefitDTO
            {
                Id = Guid.NewGuid(),
                Limit = benefit.Limit,
                Attribute = benefit.Attribute,
            };
            return clone;
        }

    }
}

