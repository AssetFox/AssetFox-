using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs
{
    public class AddPoorCountGraphTab
    {
        private PoorBridgeCount _poorBridgeCount;

        public AddPoorCountGraphTab()
        {
            _poorBridgeCount = new PoorBridgeCount();
        }

        public void AddPoorCountTab(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorBridgesCountSectionYearsRow, int simulationYearsCount)
        {
            _poorBridgeCount.Fill(worksheet, bridgeWorkSummaryWorksheet, totalPoorBridgesCountSectionYearsRow, simulationYearsCount);
        }
    }
}
