using System;
using System.Collections.Generic;
using System.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using static AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums.TreatmentEnum;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment
{
    public class ScenarioSelectableTreatmentEntity : TreatmentEntity
    {
        public ScenarioSelectableTreatmentEntity()
        {
            ScenarioSelectableTreatmentScenarioBudgetJoins = new HashSet<ScenarioSelectableTreatmentScenarioBudgetEntity>();
            ScenarioTreatmentConsequences = new HashSet<ScenarioConditionalTreatmentConsequenceEntity>();
            ScenarioTreatmentCosts = new HashSet<ScenarioTreatmentCostEntity>();
            ScenarioTreatmentPerformanceFactors = new HashSet<ScenarioTreatmentPerformanceFactorEntity>();
            ScenarioTreatmentSchedulings = new HashSet<ScenarioTreatmentSchedulingEntity>();
            ScenarioTreatmentSupersedeRules = new HashSet<ScenarioTreatmentSupersedeRuleEntity>();
        }
        public string Description { get; set; }

        public Guid SimulationId { get; set; }

        public TreatmentCategory Category { get; set; }

        public string AssetType { get; set; }

        public bool IsModified { get; set; }

        public Guid LibraryId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public bool IsUnselectable { get; set; }

        public virtual CriterionLibraryScenarioSelectableTreatmentEntity CriterionLibraryScenarioSelectableTreatmentJoin { get; set; }
        public virtual ICollection<ScenarioSelectableTreatmentScenarioBudgetEntity> ScenarioSelectableTreatmentScenarioBudgetJoins { get; set; }
        public virtual ICollection<ScenarioConditionalTreatmentConsequenceEntity> ScenarioTreatmentConsequences { get; set; }
        public virtual ICollection<ScenarioTreatmentCostEntity> ScenarioTreatmentCosts { get; set; }
        public virtual ICollection<ScenarioTreatmentPerformanceFactorEntity> ScenarioTreatmentPerformanceFactors { get; set; }
        public virtual ICollection<ScenarioTreatmentSchedulingEntity> ScenarioTreatmentSchedulings { get; set; }
        public virtual ICollection<ScenarioTreatmentSupersedeRuleEntity> ScenarioTreatmentSupersedeRules { get; set; }
    }
}
