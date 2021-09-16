using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AppliedResearchAssociates.iAM.Domains.SelectableTreatment;

namespace BridgeCareCore.Models.SummaryReport
{
    public class WorkTypeTotalAggregated
    {
        public Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> WorkTypeTotalCulvert;
        public Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> WorkTypeTotalBridge;
    }
}
