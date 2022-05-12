using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport
{
    public interface IAddPoorDeckAreaGraphTab
    {
        void AddPoorDeckAreaTab(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorBridgesDeckAreaSectionYearsRow, int simulationYearsCount);
    }
}
