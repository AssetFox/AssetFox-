using BridgeCareCore.Interfaces.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.GraphTabs
{
    public class AddPoorCountGraphTab: IAddPoorCountGraphTab
    {
        private readonly PoorBridgeCount _poorBridgeCount;

        public AddPoorCountGraphTab(PoorBridgeCount poorBridgeCount) => _poorBridgeCount = poorBridgeCount;

        public void AddPoorCountTab(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorBridgesCountSectionYearsRow, int simulationYearsCount)
        {
            _poorBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, totalPoorBridgesCountSectionYearsRow, simulationYearsCount);
        }
    }
}
