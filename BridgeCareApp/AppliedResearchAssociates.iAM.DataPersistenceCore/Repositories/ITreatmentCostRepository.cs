using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentCostRepository
    {
        void CreateTreatmentCosts(Dictionary<Guid, List<TreatmentCost>> treatmentCostsPerTreatmentId, string simulationName);
    }
}
