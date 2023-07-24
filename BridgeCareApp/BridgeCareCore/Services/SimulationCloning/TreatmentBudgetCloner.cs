﻿using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class TreatmentBudgetCloner
    {
        internal static TreatmentBudgetDTO Clone(TreatmentBudgetDTO treatmentBudget)
        {
            var clone = new TreatmentBudgetDTO
            {
                Name = treatmentBudget.Name,
            };
            return clone;
        }

    }
}
