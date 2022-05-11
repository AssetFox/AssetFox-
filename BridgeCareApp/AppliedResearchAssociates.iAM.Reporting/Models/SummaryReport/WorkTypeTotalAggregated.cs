﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport
{
    public class WorkTypeTotalAggregated
    {
        public Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> WorkTypeTotalCulvert;
        public Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> WorkTypeTotalBridge;
        public Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> WorkTypeTotalMPMS;
    }
}
