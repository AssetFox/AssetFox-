using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.GraphTabs
{
    public class AddPoorDeckAreaGraphTab : IAddPoorDeckAreaGraphTab
    {
        private readonly PoorBridgeDeckArea _poorBridgeDeckArea;

        public AddPoorDeckAreaGraphTab(PoorBridgeDeckArea poorBridgeDeckArea) => _poorBridgeDeckArea = poorBridgeDeckArea;

        public void AddPoorDeckAreaTab(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorBridgesDeckAreaSectionYearsRow, int simulationYearsCount)
        {
            _poorBridgeDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, totalPoorBridgesDeckAreaSectionYearsRow, simulationYearsCount);
        }
    }
}
