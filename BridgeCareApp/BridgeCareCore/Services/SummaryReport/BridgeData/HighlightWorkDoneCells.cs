using System.Collections.Generic;
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
            TreatmentCause treatmentCause, int year, int index, string project, ExcelWorksheet worksheet, int row, int column)
        {
            if (treatment.Length > 0 && project.ToLower() != "no treatment")
            {
                var range = worksheet.Cells[row, column];
                ParallelBridgeBAMs(parallelBridge, treatmentCause, range);
                CashFlowedBridge(treatmentCause, range);
                if (index != 1 && (treatmentCause == TreatmentCause.CommittedProject
                    && previousYearCause == TreatmentCause.CommittedProject))
                {
                    var rangeWithPreviousColumn = worksheet.Cells[row, column - 1];
                    CommittedForConsecutiveYears(rangeWithPreviousColumn);
                    CommittedForConsecutiveYears(range);
                }
                ParallelBridgeMPMS(parallelBridge, treatmentCause, range);
                ParallelBridgeCashFlow(parallelBridge, treatmentCause, range);
            }
        }

        private void CommittedForConsecutiveYears(ExcelRange range)
        {
            _excelHelper.ApplyColor(range, Color.FromArgb(255, 153, 0));
            _excelHelper.SetTextColor(range, Color.White);
        }

        private void ParallelBridgeBAMs(int isParallel, TreatmentCause projectPickType, ExcelRange range)
        {
            if (isParallel == 1 && projectPickType == TreatmentCause.SelectedTreatment)
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
            if (projectPickType == TreatmentCause.SelectedTreatment)
            {
                _excelHelper.ApplyColor(range, Color.FromArgb(0, 255, 0));
                _excelHelper.SetTextColor(range, Color.Red);
            }
        }
    }
}
