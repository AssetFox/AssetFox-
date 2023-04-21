using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void AddSegmentMilesForBPN(ExcelWorksheet worksheet, int row, int column, List<AssetDetail> sectionDetails, BPNName bpn)
        {
            var bpnKey = bpn.ToMatchInDictionary();
                                    
            var excellentMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.OpiConditionIsExcellent());
            var goodMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.OpiConditionIsGood());
            var fairMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.OpiConditionIsFair());
            var poorMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.OpiConditionIsPoor());

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
            BPNName bpn
            )
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

            //AddSegmentMilesForBPN(worksheet, startRow, column, reportOutputData.InitialAssetSummaries, bpn);
            foreach (var yearlyData in reportOutputData.Years)
            {
                row = startRow;
                AddSegmentMilesForBPN(worksheet, row, column, yearlyData.Assets, bpn);
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
            AddOpiConditionSection(worksheet, currentCell, reportOutputData, "OPI Condition - Pavement Section Miles Statewide", chartRowsModel.OPI_StateWide_ChartModel, BPNName.Statewide);


            return chartRowsModel;
        }
    }
}
