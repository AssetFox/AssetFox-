using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AppliedResearchAssociates.iAM.Analysis.SelectableTreatment;
using static AppliedResearchAssociates.iAM.Analysis.TreatmentCategories;

namespace BridgeCareCore.Models.SummaryReport
{
    public class WorkTypeTotalAggregated
    {
        public Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> WorkTypeTotalCulvert;
        public Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> WorkTypeTotalBridge;
        public Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> WorkTypeTotalMPMS;
    }
}
