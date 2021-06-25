using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.GraphTabs.BPN
{
    public class AddBPNGraphTab : IAddBPNGraphTab
    {
        private readonly PoorBridgeDeckAreaByBPN _poorBridgeDeckAreaByBPN;

        private readonly PostedBPNCount _postedBPNCount;
        private readonly PostedBPNDeckArea _postedBPNDeckArea;
        private readonly ClosedBPNCount _closedBPNCount;
        private readonly ClosedBPNDeckArea _closedBPNDeckArea;
        private readonly CombinedPostedAndClosed _combinedPostedAndClosed;
        private readonly CashNeededByBPN _cashNeededByBPN;

        public AddBPNGraphTab(PoorBridgeDeckAreaByBPN poorBridgeDeckAreaByBPN, PostedBPNCount postedBPNCount,
            PostedBPNDeckArea postedBPNDeckArea, ClosedBPNCount closedBPNCount, ClosedBPNDeckArea closedBPNDeckArea,
            CombinedPostedAndClosed combinedPostedAndClosed, CashNeededByBPN cashNeededByBPN)
        {
            _poorBridgeDeckAreaByBPN = poorBridgeDeckAreaByBPN ?? throw new ArgumentNullException(nameof(poorBridgeDeckAreaByBPN));

            _postedBPNCount = postedBPNCount ?? throw new ArgumentNullException(nameof(postedBPNCount));
            _postedBPNDeckArea = postedBPNDeckArea ?? throw new ArgumentNullException(nameof(postedBPNDeckArea));
            _closedBPNCount = closedBPNCount ?? throw new ArgumentNullException(nameof(closedBPNCount));
            _closedBPNDeckArea = closedBPNDeckArea ?? throw new ArgumentNullException(nameof(closedBPNDeckArea));
            _combinedPostedAndClosed = combinedPostedAndClosed ?? throw new ArgumentNullException(nameof(combinedPostedAndClosed));
            _cashNeededByBPN = cashNeededByBPN ?? throw new ArgumentNullException(nameof(cashNeededByBPN));
        }
        public void AddBPNTab(ExcelPackage excelPackage, ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorksheet, ChartRowsModel chartRowModel, int simulationYearsCount)
        {
            // Poor Bridge DA By BPN TAB
            worksheet = excelPackage.Workbook.Worksheets.Add("Poor DA By BPN");
            _poorBridgeDeckAreaByBPN.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPoorDeckAreaByBPNSectionYearsRow, simulationYearsCount);

            // Posted BPN count TAB
            worksheet = excelPackage.Workbook.Worksheets.Add("Posted BPN Count");
            _postedBPNCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalBridgePostedCountByBPNYearsRow, simulationYearsCount);

            // Posted BPN DA TAB
            worksheet = excelPackage.Workbook.Worksheets.Add("Posted BPN DA");
            _postedBPNDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPostedBridgeDeckAreaByBPNYearsRow, simulationYearsCount);

            // Closed BPN Count TAB
            worksheet = excelPackage.Workbook.Worksheets.Add("Closed BPN Count");
            _closedBPNCount.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalClosedBridgeCountByBPNYearsRow, simulationYearsCount);

            // Closed BPN Deck Area TAB
            worksheet = excelPackage.Workbook.Worksheets.Add("Closed BPN DA");
            _closedBPNDeckArea.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalClosedBridgeDeckAreaByBPNYearsRow, simulationYearsCount);

            // Combined Posted and Closed TAB
            worksheet = excelPackage.Workbook.Worksheets.Add("Combined Posted and Closed");
            _combinedPostedAndClosed.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalPostedAndClosedByBPNYearsRow, simulationYearsCount);

            // $ Needed DA BPN TAB
            worksheet = excelPackage.Workbook.Worksheets.Add("$ Needed DA BPN");
            _cashNeededByBPN.Fill(worksheet, bridgeWorkSummaryWorksheet, chartRowModel.TotalCashNeededByBPNYearsRow, simulationYearsCount);
        }
    }
}
