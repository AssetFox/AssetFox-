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
        public SimulationYearDetailEntity()
        {

        }
        public Guid Id { get; set; }

        public Guid SimulationOutputId { get; set; }
        public virtual SimulationOutputEntity SimulationOutput { get; set; }
        public double ConditionOfNetwork { get; set; }
        public int Year { get; set; }
        public ICollection<BudgetDetailEntity> Budgets { get; set; } = new HashSet<BudgetDetailEntity>();
        public ICollection<DeficientConditionGoalDetailEntity> DeficientConditionGoals { get; } = new HashSet<DeficientConditionGoalDetailEntity>();

        public ICollection<AssetDetailEntity> Assets { get; } = new HashSet<AssetDetailEntity>();

        public ICollection<TargetConditionGoalDetailEntity> TargetConditionGoals { get; } = new List<TargetConditionGoalDetailEntity>();


    }
}
