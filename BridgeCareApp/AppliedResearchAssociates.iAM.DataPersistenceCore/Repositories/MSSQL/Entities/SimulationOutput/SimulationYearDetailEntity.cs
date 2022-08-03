using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SimulationYearDetailEntity: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid SimulationOutputId { get; set; }
        public virtual SimulationOutputEntity SimulationOutput { get; set; }
        public double ConditionOfNetwork { get; set; }
        public int Year { get; set; }
        //        public List<BudgetDetail> Budgets { get; } = new List<BudgetDetail>();
        //public List<DeficientConditionGoalDetail> DeficientConditionGoals { get; } = new List<DeficientConditionGoalDetail>();

        //public List<AssetDetail> Assets { get; } = new List<AssetDetail>();

        //public List<TargetConditionGoalDetail> TargetConditionGoals { get; } = new List<TargetConditionGoalDetail>();


    }
}
