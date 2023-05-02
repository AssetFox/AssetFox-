using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class InventoryParameters
    {
        public int BRKey { get; set; }
        public string BMSID { get; set; }
    }

    public class InventoryParams
    {
       public List<string> keyProperties { get; set; }
    }
}
