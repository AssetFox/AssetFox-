﻿using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal
{
    public class TargetConditionGoalEntity : ConditionGoalEntity
    {
        public Guid TargetConditionGoalLibraryId { get; set; }

        public double Target { get; set; }

        public int? Year { get; set; }

        public virtual TargetConditionGoalLibraryEntity TargetConditionGoalLibrary { get; set; }

        public virtual CriterionLibraryTargetConditionGoalEntity CriterionLibraryTargetConditionGoalJoin { get; set; }
    }
}
