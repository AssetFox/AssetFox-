using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class DeficientConditionGoalEntity : ConditionGoalEntity
    {
        public Guid DeficientConditionGoalLibraryId { get; set; }

        public double AllowedDeficientPercentage { get; set; }

        public double DeficientLimit { get; set; }

        public virtual DeficientConditionGoalLibraryEntity DeficientConditionGoalLibrary { get; set; }

        public virtual CriterionLibraryDeficientConditionGoalEntity CriterionLibraryDeficientConditionGoalJoin { get; set; }
    }
}
