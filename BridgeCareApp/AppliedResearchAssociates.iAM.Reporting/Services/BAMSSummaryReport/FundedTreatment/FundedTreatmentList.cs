using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.FundedTreatment
{
    public class FundedTreatmentList : IFundedTreatmentList
    {
        private IUnfundedTreatmentCommon _unfundedTreatmentCommon;
        private ISummaryReportHelper _summaryReportHelper;

        public FundedTreatmentList()
        {
            _unfundedTreatmentCommon = new UnfundedTreatmentCommon.UnfundedTreatmentCommon();
            _summaryReportHelper = new SummaryReportHelper();
        }

        public void Fill(ExcelWorksheet unfundedRecommendationWorksheet, SimulationOutput simulationOutput)
        {

        }
    }
}
