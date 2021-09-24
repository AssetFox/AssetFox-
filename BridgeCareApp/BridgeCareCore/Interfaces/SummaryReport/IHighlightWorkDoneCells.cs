using AppliedResearchAssociates.iAM.Analysis.Engine;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IHighlightWorkDoneCells
    {
        void CheckConditions(int parallelBridge, string treatment, string previousYearTreatment, TreatmentCause previousYearCause, TreatmentCause treatmentCause,
            int year, int index, ExcelWorksheet worksheet, int row, int column);
    }
}
