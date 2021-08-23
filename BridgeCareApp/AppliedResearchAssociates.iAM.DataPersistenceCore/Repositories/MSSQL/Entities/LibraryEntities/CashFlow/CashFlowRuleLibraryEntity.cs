﻿using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow
{
    public class CashFlowRuleLibraryEntity : LibraryEntity
    {
        public CashFlowRuleLibraryEntity()
        {
            CashFlowRules = new HashSet<CashFlowRuleEntity>();
        }

        public virtual ICollection<CashFlowRuleEntity> CashFlowRules { get; set; }
    }
}