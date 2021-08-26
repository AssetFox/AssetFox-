using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment
{
    public class ScenarioConditionalTreatmentConsequenceEquationEntity : BaseEquationJoinEntity
    {
        public Guid ScenarioConditionalTreatmentConsequenceId { get; set; }

        public virtual ScenarioConditionalTreatmentConsequenceEntity ScenarioConditionalTreatmentConsequence { get; set; }
    }
}
