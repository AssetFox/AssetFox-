using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public class BaseCashFlowRuleEntity : BaseEntity
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
    }
}
