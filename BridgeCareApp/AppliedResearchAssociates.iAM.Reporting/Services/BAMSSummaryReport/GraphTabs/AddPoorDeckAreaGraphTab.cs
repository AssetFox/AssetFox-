using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs
{
    public class AddPoorDeckAreaGraphTab : IAddPoorDeckAreaGraphTab
    {
        private PoorBridgeDeckArea _poorBridgeDeckArea;

        public AddPoorDeckAreaGraphTab()
        {
            _poorBridgeDeckArea = new PoorBridgeDeckArea();
        }

        public void AddPoorDeckAreaTab(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorBridgesDeckAreaSectionYearsRow, int simulationYearsCount)
        {
            _poorBridgeDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, totalPoorBridgesDeckAreaSectionYearsRow, simulationYearsCount);
        }
    }
}
