using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.Helpers
{
    public class DatumLog
    {
        public Guid datumId { get; set; }
        public Guid locationId { get; set; }
        public string datumName { get; set; }

        public DatumLog(Guid dataumId, Guid locationId, string datumName)
        {
            this.datumId = dataumId;
            this.locationId = locationId;
            this.datumName = datumName;
        }
        public override string ToString()
        {
            return datumName + "," + locationId + "," + datumId;
        }
    }
}
