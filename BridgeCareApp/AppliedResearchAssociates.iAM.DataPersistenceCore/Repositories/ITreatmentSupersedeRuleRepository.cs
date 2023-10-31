using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentSupersedeRuleRepository
    {
        void CreateTreatmentSupersedeRules(
            Dictionary<Guid, List<TreatmentSupersedeRule>> treatmentSupersedeRulesPerTreatmentId, string simulationName, Guid simulationId);
    }
}
