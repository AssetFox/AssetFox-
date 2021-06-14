using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IUnfundedTreatmentFinalList
    {
        void Fill(ExcelWorksheet unfundedRecommendationWorksheet, SimulationOutput simulationOutput);
    }
}
