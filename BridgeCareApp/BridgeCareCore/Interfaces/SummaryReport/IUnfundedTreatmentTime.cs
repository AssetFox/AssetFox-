using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IUnfundedTreatmentTime
    {
        void Fill(ExcelWorksheet unfundedRecommendationWorksheet, SimulationOutput simulationOutput);
    }
}
