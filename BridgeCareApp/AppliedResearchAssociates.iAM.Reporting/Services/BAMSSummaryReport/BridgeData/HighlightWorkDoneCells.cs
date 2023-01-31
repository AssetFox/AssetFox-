using System.Drawing;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeData
{
    public class HighlightWorkDoneCells : IHighlightWorkDoneCells
    {
        public void CheckConditions(int parallelBridge, string treatment, string previousYearTreatment, TreatmentCause previousYearCause,
            TreatmentCause treatmentCause, int year, int index, ExcelWorksheet worksheet, int row, int column)
        {
            if (treatment != null && treatment.ToLower() != BAMSConstants.NoTreatment)
            {
                var range = worksheet.Cells[row, column, row, column + 1];
                var rangeForCashFlow = worksheet.Cells[row, column - 2, row, column + 1];
                //ParallelBridgeBAMs(parallelBridge, treatmentCause, range);
                CashFlowedBridge(treatmentCause, rangeForCashFlow);

                if (index != 1 && treatmentCause == TreatmentCause.CommittedProject
                    && previousYearCause == TreatmentCause.CommittedProject && previousYearTreatment.ToLower() != BAMSConstants.NoTreatment)
                {
                    var rangeWithPreviousColumn = worksheet.Cells[row, column - 1, row, column];
                    CommittedForConsecutiveYears(rangeWithPreviousColumn);
                    CommittedForConsecutiveYears(range);
                }
                //ParallelBridgeMPMS(parallelBridge, treatmentCause, range);
                //ParallelBridgeCashFlow(parallelBridge, treatmentCause, rangeForCashFlow);
            }
        }

        private void CommittedForConsecutiveYears(ExcelRange range)
        {
            ExcelHelper.ApplyColor(range, Color.FromArgb(255, 153, 0));
            ExcelHelper.SetTextColor(range, Color.White);
        }

        private void ParallelBridgeBAMs(int isParallel, TreatmentCause projectPickType, ExcelRange range)
        {
            if (isParallel == 1 && (projectPickType == TreatmentCause.SelectedTreatment ||
                projectPickType == TreatmentCause.CashFlowProject || projectPickType == TreatmentCause.CommittedProject))
            {
                ExcelHelper.ApplyColor(range, Color.FromArgb(0, 204, 255));
                ExcelHelper.SetTextColor(range, Color.Black);
            }
        }

        private void ParallelBridgeCashFlow(int isParallel, TreatmentCause projectPickType, ExcelRange range)
        {
            if (isParallel == 1 && projectPickType == TreatmentCause.CashFlowProject)
            {
                ExcelHelper.ApplyColor(range, Color.FromArgb(0, 204, 255));
                ExcelHelper.SetTextColor(range, Color.FromArgb(255, 0, 0));
                return;
            }
        }

        private void ParallelBridgeMPMS(int isParallel, TreatmentCause projectPickType, ExcelRange range)
        {
            if (isParallel == 1 && projectPickType == TreatmentCause.CommittedProject)
            {
                ExcelHelper.ApplyColor(range, Color.FromArgb(0, 204, 255));
                ExcelHelper.SetTextColor(range, Color.White);
            }
        }

        private void CashFlowedBridge(TreatmentCause projectPickType, ExcelRange range)
        {
            if (projectPickType == TreatmentCause.CashFlowProject)
            {
                ExcelHelper.ApplyColor(range, Color.FromArgb(0, 255, 0));
                ExcelHelper.SetTextColor(range, Color.Red);
            }
        }
    }
}
