using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class DeckAreaBridgeWorkSummary
    {
        private readonly BridgeWorkSummaryCommon _bridgeWorkSummaryCommon;
        private readonly IExcelHelper _excelHelper;
        private readonly BridgeWorkSummaryComputationHelper _bridgeWorkSummaryComputationHelper;

        public DeckAreaBridgeWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, IExcelHelper excelHelper, BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            _bridgeWorkSummaryCommon = bridgeWorkSummaryCommon ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryCommon));
            _excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            _bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryComputationHelper));
        }

        internal ChartRowsModel FillPoorDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears, SimulationOutput reportOutputData, ChartRowsModel chartRowsModel)
        {
            _bridgeWorkSummaryCommon.AddBridgeHeaders(worksheet, currentCell, simulationYears, "Poor Deck Area", true);
            chartRowsModel.TotalPoorDeckAreaByBPNSectionYearsRow = currentCell.Row;
            AddDetailsForPoorDeckArea(worksheet, currentCell, reportOutputData);
            return chartRowsModel;
        }

        #region

        private void AddDetailsForPoorDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell,
            SimulationOutput reportOutputData)
        {
            int startRow, startColumn, row, column;
            _bridgeWorkSummaryCommon.InitializeBPNLabels(worksheet, currentCell, out startRow, out startColumn, out row, out column);
            AddInitialPoorDeckArea(worksheet, startRow, column, reportOutputData.InitialSectionSummaries);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                column = ++column;
                AddPoorDeckArea(worksheet, row, column, yearlyData.Sections);
            }
            _excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row + 3, column]);
            _bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, row + 4, column);
        }

        private void AddInitialPoorDeckArea(ExcelWorksheet worksheet, int row, int column,
            List<SectionSummaryDetail> initialSectionSummaries)
        {
            var postedBridgesBpn1 = initialSectionSummaries.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == "1");
            var selectedBridgesBpn1 = postedBridgesBpn1.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] < 5);
            var poorDeckAreaBpn1 = selectedBridgesBpn1.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);

            worksheet.Cells[row++, column].Value = poorDeckAreaBpn1;

            var postedBridgesBpn2H = initialSectionSummaries.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == "2"
            || b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == "H");
            var selectedBridgesBpn2H = postedBridgesBpn2H.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] < 5);
            var poorDeckAreaBpn2H = selectedBridgesBpn2H.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);

            worksheet.Cells[row++, column].Value = poorDeckAreaBpn2H;

            var postedBridgesBpn3 = initialSectionSummaries.FindAll(b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] == "3");
            var selectedBridgesBpn3 = postedBridgesBpn1.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] < 5);
            var poorDeckAreaBpn3 = selectedBridgesBpn3.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);

            worksheet.Cells[row++, column].Value = poorDeckAreaBpn3;

            var postedBridgesRemainingBpn = initialSectionSummaries.FindAll(
                b => b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "2" &&
                b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "H" && b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "1" &&
                b.ValuePerTextAttribute["BUS_PLAN_NETWORK"] != "3"
            );
            var selectedBridgesRemainingBpn = postedBridgesRemainingBpn.FindAll(_ => _.ValuePerNumericAttribute["MINCOND"] < 5);

            var poorDeckAreaRemainingBpn = selectedBridgesRemainingBpn.Sum(_ => _.ValuePerNumericAttribute["DECK_AREA"]);
            worksheet.Cells[row, column].Value = poorDeckAreaRemainingBpn;
        }

        private void AddPoorDeckArea(ExcelWorksheet worksheet, int row, int column, List<SectionDetail> sectionDetails)
        {
            var poorDeckArea = _bridgeWorkSummaryComputationHelper.CalculatePoorDeckAreaForBPN13(sectionDetails, "1");
            worksheet.Cells[row++, column].Value = poorDeckArea;
            poorDeckArea = _bridgeWorkSummaryComputationHelper.CalculatePoorDeckAreaForBPN2H(sectionDetails);
            worksheet.Cells[row++, column].Value = poorDeckArea;
            poorDeckArea = _bridgeWorkSummaryComputationHelper.CalculatePoorDeckAreaForBPN13(sectionDetails, "3");
            worksheet.Cells[row++, column].Value = poorDeckArea;
            poorDeckArea = _bridgeWorkSummaryComputationHelper.CalculatePoorDeckAreaForRemainingBPN(sectionDetails);
            worksheet.Cells[row, column].Value = poorDeckArea;
        }

        #endregion
    }
}
