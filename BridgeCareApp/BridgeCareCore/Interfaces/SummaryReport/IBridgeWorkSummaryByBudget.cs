﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IBridgeWorkSummaryByBudget
    {
        public void Fill(ExcelWorksheet summaryByBudgetWorksheet, SimulationOutput reportOutputData, List<int> simulationYears);
    }
}
