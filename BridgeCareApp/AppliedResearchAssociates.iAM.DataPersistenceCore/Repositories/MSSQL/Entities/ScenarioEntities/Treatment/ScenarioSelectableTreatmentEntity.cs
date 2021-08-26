using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment
{
    public class ScenarioSelectableTreatmentEntity : TreatmentEntity
    {
        public ScenarioSelectableTreatmentEntity()
        {
            ScenarioSelectableTreatmentScenarioBudgetJoins = new HashSet<ScenarioSelectableTreatmentScenarioBudgetEntity>();
            ScenarioTreatmentConsequences = new HashSet<ScenarioConditionalTreatmentConsequenceEntity>();
            ScenarioTreatmentCosts = new HashSet<ScenarioTreatmentCostEntity>();
            ScenarioTreatmentSchedulings = new HashSet<ScenarioTreatmentSchedulingEntity>();
            ScenarioTreatmentSupersessions = new HashSet<ScenarioTreatmentSupersessionEntity>();
        }
        public string Description { get; set; }

        public Guid SimulationId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual CriterionLibraryScenarioSelectableTreatmentEntity CriterionLibraryScenarioSelectableTreatmentJoin { get; set; }
        public virtual ICollection<ScenarioSelectableTreatmentScenarioBudgetEntity> ScenarioSelectableTreatmentScenarioBudgetJoins { get; set; }
        public virtual ICollection<ScenarioConditionalTreatmentConsequenceEntity> ScenarioTreatmentConsequences { get; set; }
        public virtual ICollection<ScenarioTreatmentCostEntity> ScenarioTreatmentCosts { get; set; }
        public virtual ICollection<ScenarioTreatmentSchedulingEntity> ScenarioTreatmentSchedulings { get; set; }
        public virtual ICollection<ScenarioTreatmentSupersessionEntity> ScenarioTreatmentSupersessions { get; set; }
    }
}
