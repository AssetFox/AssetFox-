using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentSupersedeRuleRepository
    {
        void CreateTreatmentSupersedeRules(
            Dictionary<Guid, List<TreatmentSupersedeRule>> treatmentSupersedeRulesPerTreatmentId, string simulationName, Guid simulationId);

        public void UpsertOrDeleteScenarioTreatmentSupersedeRules(Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> scenarioTreatmentCostPerTreatmentId, Guid simulationId);

        public List<TreatmentSupersedeRuleDTO> GetScenarioTreatmentSupersedeRuleByTreatmentId(Guid treatmentId);
    }
}
