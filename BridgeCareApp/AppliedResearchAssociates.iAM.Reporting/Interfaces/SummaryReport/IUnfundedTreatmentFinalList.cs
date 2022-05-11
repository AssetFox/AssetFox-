using AppliedResearchAssociates.iAM.Analysis.Engine;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport
{
    public interface IUnfundedTreatmentFinalList
    {
        void Fill(ExcelWorksheet unfundedRecommendationWorksheet, SimulationOutput simulationOutput);
    }
}
