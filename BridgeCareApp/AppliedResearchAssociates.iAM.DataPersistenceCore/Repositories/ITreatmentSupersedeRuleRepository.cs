using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentSupersedeRuleRepository
    {
        public void UpsertOrDeleteScenarioTreatmentSupersedeRules(Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> scenarioTreatmentCostPerTreatmentId, Guid simulationId);

        public List<TreatmentSupersedeRuleDTO> GetScenarioTreatmentSupersedeRules(Guid treatmentId, Guid simulationId);

        public void UpsertOrDeleteTreatmentSupersedeRules(Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> supersedeRulesPerTreatmentId, Guid libraryId);

        public List<TreatmentSupersedeRuleDTO> GetTreatmentSupersedeRules(Guid treatmentId, Guid libraryId);        
    }
}
