﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class UserEntity : BaseEntity
    {
        public UserEntity()
        {
            SimulationUserJoins = new HashSet<SimulationUserEntity>();
            RemainingLifeLimitLibraryUsers = new HashSet<RemainingLifeLimitLibraryUserEntity>();
            DeficientConditionGoalLibraryUsers = new HashSet<DeficientConditionGoalLibraryUserEntity>();
            BudgetLibraryUsers = new HashSet<BudgetLibraryUserEntity>();
            BudgetPriorityLibraryUsers = new HashSet<BudgetPriorityLibraryUserEntity>();
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
        public ICollection<BudgetLibraryUserEntity> BudgetLibraryUsers { get; set; }
        public ICollection<BudgetPriorityLibraryUserEntity> BudgetPriorityLibraryUsers { get; set; }
    }
}
