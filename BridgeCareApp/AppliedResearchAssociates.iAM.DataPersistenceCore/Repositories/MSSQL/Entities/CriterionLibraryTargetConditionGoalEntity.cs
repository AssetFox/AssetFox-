using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryTargetConditionGoalEntity : BaseEntity
    {
        public Guid TargetConditionGoalId { get; set; }

        public Guid CriterionLibraryId { get; set; }

        public virtual TargetConditionGoalEntity TargetConditionGoal { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
    }
}
