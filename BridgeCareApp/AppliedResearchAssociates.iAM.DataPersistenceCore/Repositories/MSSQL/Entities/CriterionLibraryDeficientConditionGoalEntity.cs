using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryDeficientConditionGoalEntity
    {
        public Guid CriterionLibraryId { get; set; }
        public Guid DeficientConditionGoalId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
        public virtual DeficientConditionGoalEntity DeficientConditionGoal { get; set; }
    }
}
