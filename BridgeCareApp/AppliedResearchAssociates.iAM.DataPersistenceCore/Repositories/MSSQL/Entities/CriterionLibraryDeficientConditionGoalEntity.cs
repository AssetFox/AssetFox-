using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryDeficientConditionGoalEntity : BaseEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid DeficientConditionGoalId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual DeficientConditionGoalEntity DeficientConditionGoal { get; set; }
    }
}
