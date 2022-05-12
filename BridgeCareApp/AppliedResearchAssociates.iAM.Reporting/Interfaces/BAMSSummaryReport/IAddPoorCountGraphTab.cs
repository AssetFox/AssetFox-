using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport
{
    public interface IAddPoorCountGraphTab
    {
        void AddPoorCountTab(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, int totalPoorBridgesCountSectionYearsRow, int simulationYearsCount);
    }
}
