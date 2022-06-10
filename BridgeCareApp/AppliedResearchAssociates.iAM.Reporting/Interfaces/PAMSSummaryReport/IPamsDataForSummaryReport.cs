using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport
{
    public interface IPamsDataForSummaryReport
    {
        void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData);
    }
}
