using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSAuditReport
{
    internal class BridgesTab : IBridgesTab
    {
        public void Fill(ExcelWorksheet unfundedRecommendationWorksheet, SimulationOutput simulationOutput) => throw new NotImplementedException();
    }
}
