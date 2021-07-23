using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IAddPoorCountGraphTab
    {
        void AddPoorCountTab(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorBridgesCountSectionYearsRow, int simulationYearsCount);
    }
}
