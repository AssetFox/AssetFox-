using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class OpiConditionSummary
    {
        private PavementWorkSummaryComputationHelper _pavementWorkSummaryComputationHelper;
        private PavementWorkSummaryCommon _pavementWorkSummaryCommon;

        public OpiConditionSummary()
        {
            _pavementWorkSummaryCommon = new PavementWorkSummaryCommon();
            _pavementWorkSummaryComputationHelper = new PavementWorkSummaryComputationHelper();
        }

        private void AddSegmentMilesForBPN(ExcelWorksheet worksheet, int row, int column, List<AssetDetail> sectionDetails, BPNName bpn, ChartRowsModel chartRowsModel = null)
        {
            var bpnKey = bpn.ToMatchInDictionary();
            double excellentMiles = 0;
            double goodMiles = 0;
            double fairMiles = 0;
            double poorMiles = 0;

            if (bpnKey != string.Empty)
            {
                excellentMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.OpiConditionIsExcellent(bpnKey));
                goodMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.OpiConditionIsGood(bpnKey));
                fairMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.OpiConditionIsFair(bpnKey));
                poorMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.OpiConditionIsPoor(bpnKey));
            }
            else
            {
                excellentMiles = Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_1_ChartModel.sourceStartRow, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_2_ChartModel.sourceStartRow, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_3_ChartModel.sourceStartRow, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_4_ChartModel.sourceStartRow, column].Value);

                goodMiles = Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_1_ChartModel.sourceStartRow + 1, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_2_ChartModel.sourceStartRow + 1, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_3_ChartModel.sourceStartRow + 1, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_4_ChartModel.sourceStartRow + 1, column].Value);

                fairMiles = Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_1_ChartModel.sourceStartRow + 2, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_2_ChartModel.sourceStartRow + 2, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_3_ChartModel.sourceStartRow + 2, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_4_ChartModel.sourceStartRow + 2, column].Value);

                poorMiles = Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_1_ChartModel.sourceStartRow + 3, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_2_ChartModel.sourceStartRow + 3, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_3_ChartModel.sourceStartRow + 3, column].Value) +
                                 Convert.ToDouble(worksheet.Cells[chartRowsModel.OPI_BPN_4_ChartModel.sourceStartRow + 3, column].Value);
            }

            worksheet.Cells[row++, column].Value = excellentMiles;
            worksheet.Cells[row++, column].Value = goodMiles;
            worksheet.Cells[row++, column].Value = fairMiles;
            worksheet.Cells[row++, column].Value = poorMiles;
        }


        private void AddOpiConditionSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            SimulationOutput reportOutputData,
            string title,
            ChartConditionModel chartConditionModel,
            BPNName bpn,
            ChartRowsModel chartRowsModel = null)
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, reportOutputData.Years.Select(yearData => yearData.Year).ToList(), "", title);

            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var conditionLabels = new List<string> { PAMSConstants.Excellent, PAMSConstants.Good, PAMSConstants.Fair, PAMSConstants.Poor };
            _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, conditionLabels, ref row, ref column);

            chartConditionModel.sourceStartRow = startRow;

            var fromColumn = column + 2;

            row = startRow;
            column = fromColumn;
                        
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                AddSegmentMilesForBPN(worksheet, row, column, yearlyData.Assets, bpn, chartRowsModel);
                column = ++column;
            }

            column--;
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, startRow + 3, column]);
            ExcelHelper.HorizontalRightAlign(worksheet.Cells[startRow, startColumn, startRow + 3, startColumn]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, startRow + 3, column], ExcelHelperCellFormat.Number);
            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, startRow + 4, column);
        }

        public ChartRowsModel FillOpiConditionSummarySection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            SimulationOutput reportOutputData,
            ChartRowsModel chartRowsModel
            )
        {
            AddOpiConditionSection(worksheet, currentCell, reportOutputData, "OPI Condition - Pavement Section Miles BPN 1", chartRowsModel.OPI_BPN_1_ChartModel, BPNName.BPN1);
            AddOpiConditionSection(worksheet, currentCell, reportOutputData, "OPI Condition - Pavement Section Miles BPN 2", chartRowsModel.OPI_BPN_2_ChartModel, BPNName.BPN2);
            AddOpiConditionSection(worksheet, currentCell, reportOutputData, "OPI Condition - Pavement Section Miles BPN 3", chartRowsModel.OPI_BPN_3_ChartModel, BPNName.BPN3);
            AddOpiConditionSection(worksheet, currentCell, reportOutputData, "OPI Condition - Pavement Section Miles BPN 4", chartRowsModel.OPI_BPN_4_ChartModel, BPNName.BPN4);
            AddOpiConditionSection(worksheet, currentCell, reportOutputData, "OPI Condition - Pavement Section Miles Statewide", chartRowsModel.OPI_StateWide_ChartModel, BPNName.Statewide, chartRowsModel);

            return chartRowsModel;
        }
    }
}
