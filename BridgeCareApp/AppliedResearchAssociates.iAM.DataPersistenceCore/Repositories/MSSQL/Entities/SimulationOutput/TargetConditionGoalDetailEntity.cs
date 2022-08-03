using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TargetConditionGoalDetailEntity: ConditionGoalDetailEntity
    {
        // WjJake -- do we want a superclass ConditionGoalDetailEntity?
        public Guid SimulationYearDetailId { get; set; }

        public virtual SimulationYearDetailEntity SimulationYearDetail { get; set; }

        public double ActualValue { get; set; }

        public double TargetValue { get; set; }
    }
}
