using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow
{
    public class CashFlowRuleLibraryUserEntity : BaseEntity
    {
        public Guid CashFlowRuleLibraryId { get; set; }
        public Guid UserId { get; set; }
        public int AccessLevel { get; set; }
        public virtual CashFlowRuleLibraryEntity CashFlowRuleLibrary { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
