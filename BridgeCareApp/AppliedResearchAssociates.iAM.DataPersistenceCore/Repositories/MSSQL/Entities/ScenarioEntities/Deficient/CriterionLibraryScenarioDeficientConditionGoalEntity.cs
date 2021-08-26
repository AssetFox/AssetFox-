using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient
{
    public class CriterionLibraryScenarioDeficientConditionGoalEntity : BaseEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid ScenarioDeficientConditionGoalId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual ScenarioDeficientConditionGoalEntity ScenarioDeficientConditionGoal { get; set; }
    }
}
