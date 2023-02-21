using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class UserEntity : BaseEntity
    {
        public UserEntity()
        {
            SimulationUserJoins = new HashSet<SimulationUserEntity>();
            RemainingLifeLimitLibraryUsers = new HashSet<RemainingLifeLimitLibraryUserEntity>();
            DeficientConditionGoalLibraryUsers = new HashSet<DeficientConditionGoalLibraryUserEntity>();
            CalculatedAttributeLibraryUsers= new HashSet<CalculatedAttributeLibraryUserEntity>();
            CashFlowRuleLibraryUsers = new HashSet<CashFlowRuleLibraryUserEntity>();
            BudgetLibraryUsers = new HashSet<BudgetLibraryUserEntity>();
        }

        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool HasInventoryAccess { get; set; }

        public DateTime LastNewsAccessDate { get; set; }

        public virtual CriterionLibraryUserEntity CriterionLibraryUserJoin { get; set; }

        public virtual UserCriteriaFilterEntity UserCriteriaFilterJoin { get; set; }

        public ICollection<SimulationUserEntity> SimulationUserJoins { get; set; }

        public ICollection<RemainingLifeLimitLibraryUserEntity> RemainingLifeLimitLibraryUsers { get; set; }
        public ICollection<DeficientConditionGoalLibraryUserEntity> DeficientConditionGoalLibraryUsers { get; set; }
        public ICollection<CashFlowRuleLibraryUserEntity> CashFlowRuleLibraryUsers { get; set; }
        public ICollection<CalculatedAttributeLibraryUserEntity> CalculatedAttributeLibraryUsers { get; set; }
        public ICollection<BudgetLibraryUserEntity> BudgetLibraryUsers { get; set; }
    }
}
