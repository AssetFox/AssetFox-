using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistence.Models;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL
{
    public class InvestmentRepository : GenericMSSQLRepository<Investments>
    {
        public InvestmentRepository(BridgeCareContext context) : base(context)
        {

        }
    }
}
