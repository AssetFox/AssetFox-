using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CashFlowConsiderationDetailEntity
    {
        public Guid TreatmentConsiderationDetailId { get; set; }

        public virtual TreatmentConsiderationDetailEntity TreatmentConsiderationDetail { get; set; }

        public string CashFlowRuleName { get; }

        public ReasonAgainstCashFlow ReasonAgainstCashFlow { get; set; }
    }
}
