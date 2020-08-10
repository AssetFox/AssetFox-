using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistence.DomainModels;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories
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
