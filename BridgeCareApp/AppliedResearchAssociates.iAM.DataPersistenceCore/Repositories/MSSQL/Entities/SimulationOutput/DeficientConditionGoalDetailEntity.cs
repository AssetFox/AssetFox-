using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class DeficientConditionGoalDetailEntity: BaseEntity
    {
        // WjJake -- do we want a superclass ConditionGoalDetailEntity? // YES
        public Guid SimulationYearDetailId { get; set; }
        public virtual SimulationYearDetailEntity SimulationYearDetail { get; set; }

        public double ActualDeficientPercentage { get; set; }

        public double AllowedDeficientPercentage { get; set; }

        public double DeficientLimit { get; set; }

        // Below are from ConditionGoalDetail
        public Guid AttributeId { get; set; }

        public bool GoalIsMet { get; set; }

        public string GoalName { get; set; }
    }
}
