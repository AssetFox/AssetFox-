using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment
{
    public class ScenarioTreatmentSupersedeRuleEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid TreatmentId { get; set; }

        public virtual ScenarioSelectableTreatmentEntity ScenarioSelectableTreatment { get; set; }

        public virtual ScenarioSelectableTreatmentEntity PreventScenarioSelectableTreatment { get; set; }

        public virtual CriterionLibraryScenarioTreatmentSupersedeRuleEntity CriterionLibraryScenarioTreatmentSupersedeRuleJoin { get; set; }
    }
}
