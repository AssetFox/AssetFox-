using System.Drawing;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.BridgeData
{
    public class HighlightWorkDoneCells : IHighlightWorkDoneCells
    {
        private readonly IExcelHelper _excelHelper;

        public HighlightWorkDoneCells(IExcelHelper excelHelper)
        {
            _excelHelper = excelHelper;
        }

        public void CheckConditions(int parallelBridge, string treatment, TreatmentCause previousYearCause,
            TreatmentCause treatmentCause, int year, int index, ExcelWorksheet worksheet, int row, int column)
        {
            if (treatment != null && treatment.ToLower() != Properties.Resources.NoTreatment)
            {
                var range = worksheet.Cells[row, column];
                var rangeForCashFlow = worksheet.Cells[row, column - 1, row, column];
                ParallelBridgeBAMs(parallelBridge, treatmentCause, range);
                CashFlowedBridge(treatmentCause, rangeForCashFlow);

                if (index != 1 && treatmentCause == TreatmentCause.CommittedProject
                    && previousYearCause == TreatmentCause.CommittedProject)
                {
                    var rangeWithPreviousColumn = worksheet.Cells[row, column - 1];
                    CommittedForConsecutiveYears(rangeWithPreviousColumn);
                    CommittedForConsecutiveYears(range);
                }
                ParallelBridgeMPMS(parallelBridge, treatmentCause, range);
                ParallelBridgeCashFlow(parallelBridge, treatmentCause, rangeForCashFlow);
            }
        }

        private void CommittedForConsecutiveYears(ExcelRange range)
        {
            _excelHelper.ApplyColor(range, Color.FromArgb(255, 153, 0));
            _excelHelper.SetTextColor(range, Color.White);
        }

        private void ParallelBridgeBAMs(int isParallel, TreatmentCause projectPickType, ExcelRange range)
        {
            if (isParallel == 1 && (projectPickType == TreatmentCause.SelectedTreatment ||
                projectPickType == TreatmentCause.CashFlowProject || projectPickType == TreatmentCause.CommittedProject))
            {
                _excelHelper.ApplyColor(range, Color.FromArgb(0, 204, 255));
                _excelHelper.SetTextColor(range, Color.Black);
            }
        }

        private void ParallelBridgeCashFlow(int isParallel, TreatmentCause projectPickType, ExcelRange range)
        {
            if (isParallel == 1 && projectPickType == TreatmentCause.CashFlowProject)
            {
                _excelHelper.ApplyColor(range, Color.FromArgb(0, 204, 255));
                _excelHelper.SetTextColor(range, Color.FromArgb(255, 0, 0));
                return;
            }
        }

        private void ParallelBridgeMPMS(int isParallel, TreatmentCause projectPickType, ExcelRange range)
        {
            if (isParallel == 1 && projectPickType == TreatmentCause.CommittedProject)
            {
                _excelHelper.ApplyColor(range, Color.FromArgb(0, 204, 255));
                _excelHelper.SetTextColor(range, Color.White);
            }
        }

        private void CashFlowedBridge(TreatmentCause projectPickType, ExcelRange range)
        {
            if (projectPickType == TreatmentCause.CashFlowProject)
            {
                _excelHelper.ApplyColor(range, Color.FromArgb(0, 255, 0));
                _excelHelper.SetTextColor(range, Color.Red);
            }
        }
    }
}
