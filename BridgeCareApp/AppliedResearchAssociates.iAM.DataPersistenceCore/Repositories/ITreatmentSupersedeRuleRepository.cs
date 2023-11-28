using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentSupersedeRuleRepository
    {
        public void UpsertOrDeleteScenarioTreatmentSupersedeRules(Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> scenarioTreatmentCostPerTreatmentId, Guid simulationId);

        public List<TreatmentSupersedeRuleDTO> GetScenarioTreatmentSupersedeRules(Guid treatmentId, Guid simulationId);

        public List<TreatmentSupersedeRuleExportDTO> GetScenarioTreatmentSupersedeRulesBysimulationId(Guid simulationId);

        public void UpsertOrDeleteTreatmentSupersedeRules(Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> supersedeRulesPerTreatmentId, Guid libraryId);

        public List<TreatmentSupersedeRuleDTO> GetLibraryTreatmentSupersedeRules(Guid treatmentId, Guid libraryId);

        public List<TreatmentSupersedeRuleExportDTO> GetLibraryTreatmentSupersedeRulesByLibraryId(Guid libraryId);
    }
}
