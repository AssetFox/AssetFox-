using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class NetworkExportReportModel
    {
        public Guid MaintainableAssetID { get; set; }

        public Dictionary<string, string> Attributes { get; set; }
    }
}
