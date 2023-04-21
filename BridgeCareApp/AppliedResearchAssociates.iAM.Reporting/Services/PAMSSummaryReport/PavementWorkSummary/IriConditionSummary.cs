using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class IriConditionSummary
    {
        private PavementWorkSummaryCommon _pavementWorkSummaryCommon;
        private PavementWorkSummaryComputationHelper _pavementWorkSummaryComputationHelper;

        public IriConditionSummary()
        {
            _pavementWorkSummaryCommon = new PavementWorkSummaryCommon();
            _pavementWorkSummaryComputationHelper = new PavementWorkSummaryComputationHelper();
        }

        private void AddSegmentMilesForBPN(ExcelWorksheet worksheet, int row, int column, List<AssetDetail> sectionDetails, BPNName bpn)
        {
            var bpnKey = bpn.ToMatchInDictionary();
                        
            var excellentMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.IriConditionIsExcellent());
            var goodMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.IriConditionIsGood());
            var fairMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.IriConditionIsFair());
            var poorMiles = _pavementWorkSummaryComputationHelper.CalculateSegmentMilesForBPNWithCondition(sectionDetails, bpnKey, _ => _.IriConditionIsPoor());

            worksheet.Cells[row++, column].Value = excellentMiles;
            worksheet.Cells[row++, column].Value = goodMiles;
            worksheet.Cells[row++, column].Value = fairMiles;
            worksheet.Cells[row++, column].Value = poorMiles;
        }

        private void AddIriConditionSection(
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

        public ChartRowsModel FillIriConditionSummarySection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            SimulationOutput reportOutputData,
            ChartRowsModel chartRowsModel
            )
        {
            AddIriConditionSection(worksheet, currentCell, reportOutputData, "IRI Condition - Pavement Section Miles BPN 1", chartRowsModel.IRI_BPN_1_ChartModel, BPNName.BPN1);
            AddIriConditionSection(worksheet, currentCell, reportOutputData, "IRI Condition - Pavement Section Miles BPN 2", chartRowsModel.IRI_BPN_2_ChartModel, BPNName.BPN2);
            AddIriConditionSection(worksheet, currentCell, reportOutputData, "IRI Condition - Pavement Section Miles BPN 3", chartRowsModel.IRI_BPN_3_ChartModel, BPNName.BPN3);
            AddIriConditionSection(worksheet, currentCell, reportOutputData, "IRI Condition - Pavement Section Miles BPN 4", chartRowsModel.IRI_BPN_4_ChartModel, BPNName.BPN4);
            AddIriConditionSection(worksheet, currentCell, reportOutputData, "IRI Condition - Pavement Section Miles Statewide", chartRowsModel.IRI_StateWide_ChartModel, BPNName.Statewide);

            return chartRowsModel;
        }
    }
}
