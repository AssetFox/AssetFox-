﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class UserEntity : BaseEntity
    {
        public UserEntity()
        {
            SimulationUserJoins = new HashSet<SimulationUserEntity>();
            BudgetLibraryUsers = new HashSet<BudgetLibraryUserEntity>();
        }

        public Guid Id { get; set; }

        public string Username { get; set; }

        public bool HasInventoryAccess { get; set; }

        public DateTime LastNewsAccessDate { get; set; }

        public virtual CriterionLibraryUserEntity CriterionLibraryUserJoin { get; set; }

        public virtual UserCriteriaFilterEntity UserCriteriaFilterJoin { get; set; }

        public ICollection<SimulationUserEntity> SimulationUserJoins { get; set; }
        public ICollection<BudgetLibraryUserEntity> BudgetLibraryUsers { get; set; }
    }
}
