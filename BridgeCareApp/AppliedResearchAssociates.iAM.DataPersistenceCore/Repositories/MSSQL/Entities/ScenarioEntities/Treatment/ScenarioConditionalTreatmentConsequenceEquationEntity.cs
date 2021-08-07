using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities
{
    public class ScenarioConditionalTreatmentConsequenceEquationEntity : BaseEquationJoinEntity
    {
        public Guid ScenarioConditionalTreatmentConsequenceId { get; set; }

        public virtual ScenarioConditionalTreatmentConsequenceEntity ScenarioConditionalTreatmentConsequence { get; set; }
    }
}
