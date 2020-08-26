using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistence.Models;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL
{
    public class InventoryRepository : GenericMSSQLRepository<Inventory>
    {
        public InventoryRepository(BridgeCareContext context) : base(context)
        {

        }
    }

    public class BridgeCareContext
    {
    }
}
