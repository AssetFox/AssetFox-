using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IAddPoorDeckAreaGraphTab
    {
        void AddPoorDeckAreaTab(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorBridgesDeckAreaSectionYearsRow, int simulationYearsCount);
    }
}
