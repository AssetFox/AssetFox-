using System.Drawing;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeData
{
    public class HighlightWorkDoneCells
    {
        public void CheckConditions(string treatment, string previousYearTreatment, TreatmentCause previousYearCause,
            TreatmentCause treatmentCause, int index, ExcelWorksheet worksheet, int row, int column, TreatmentStatus treatmentStatus, TreatmentStatus previousYearTreatmentStatus)
        {
            if (treatment != null && treatment.ToLower() != BAMSConstants.NoTreatment)
            {
                var range = worksheet.Cells[row, column, row, column + 1];
                var rangeForCashFlow = worksheet.Cells[row, column - 2, row, column + 1];
                CashFlowedBridge(treatmentCause, rangeForCashFlow);

                if (index != 1 && CommittedProjectsCashFlowed(treatment, previousYearTreatment, previousYearCause, treatmentCause, treatmentStatus, previousYearTreatmentStatus))
                {
                    var rangeWithPreviousColumn = worksheet.Cells[row, column - 2, row, column - 1];
                    CommittedForConsecutiveYears(rangeWithPreviousColumn);
                    CommittedForConsecutiveYears(range);
                }
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

        public static bool CommittedProjectsCashFlowed(string treatment, string previousYearTreatment, TreatmentCause previousYearCause, TreatmentCause treatmentCause, TreatmentStatus treatmentStatus, TreatmentStatus previousYearTreatmentStatus)
        {
            return treatment != null && previousYearTreatment != null
                   && treatment.ToLower() != PAMSConstants.NoTreatment && treatment == previousYearTreatment
                   && treatmentCause == TreatmentCause.CommittedProject
                   && previousYearCause == TreatmentCause.CommittedProject
                   && previousYearTreatmentStatus == TreatmentStatus.Progressed
                   && (treatmentStatus == TreatmentStatus.Progressed || treatmentStatus == TreatmentStatus.Applied);
        }
    }
}
