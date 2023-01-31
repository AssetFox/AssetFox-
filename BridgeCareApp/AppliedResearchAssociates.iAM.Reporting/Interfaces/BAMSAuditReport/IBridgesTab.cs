using AppliedResearchAssociates.iAM.Analysis.Engine;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSAuditReport
{
    public interface IBridgesTab
    {
        void Fill(ExcelWorksheet bridgesWorksheet, SimulationOutput simulationOutput);
    }
}
