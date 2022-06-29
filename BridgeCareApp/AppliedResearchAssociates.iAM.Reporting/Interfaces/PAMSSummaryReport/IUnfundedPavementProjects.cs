using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport
{
    public interface IUnfundedPavementProjects
    {
        void Fill(ExcelWorksheet unfundedRecommendationWorksheet, SimulationOutput simulationOutput);
    }
}
