using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Reporting
{
    /// <summary>
    /// Domain object representing record in data persistence for report data that should be persisted
    /// </summary>
    public class ReportIndex
    {
        public Guid ID { get; set; }
        public Guid? SimulationID { get; set; }
        public string ReportTypeName { get; set; }
        public string Result { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
