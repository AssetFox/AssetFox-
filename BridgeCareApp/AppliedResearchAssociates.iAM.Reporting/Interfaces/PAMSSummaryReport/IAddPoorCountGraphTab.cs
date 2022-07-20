using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport
{
    public interface IAddPoorCountGraphTab
    {
        void AddPoorCountTab(ExcelWorksheet worksheet, ExcelWorksheet pamsWorkSummaryWorksheet, int totalPoorCountSectionYearsRow, int simulationYearsCount);
    }
}
