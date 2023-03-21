using AppliedResearchAssociates.iAM.Analysis.Engine;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSPBExportReport
{
    public interface ITreatmentForPBExportReport
    {
        void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData);
    }
}
