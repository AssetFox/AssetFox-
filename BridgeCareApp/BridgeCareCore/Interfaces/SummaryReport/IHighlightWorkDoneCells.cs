using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IHighlightWorkDoneCells
    {
        void CheckConditions(int parallelBridge, string treatment, TreatmentCause previousYearCause, TreatmentCause treatmentCause, int year, int index, ExcelWorksheet worksheet, int row, int column);
    }
}
