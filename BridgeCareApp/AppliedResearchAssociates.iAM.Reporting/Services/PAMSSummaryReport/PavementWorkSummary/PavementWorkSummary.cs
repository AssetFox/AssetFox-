using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class PavementWorkSummary
    {
        private CostBudgetsWorkSummary _costBudgetsWorkSummary;
        private TreatmentsWorkSummary _treatmentsWorkSummary;
        private IriConditionSummary _iriConditionSummary;
        private OpiConditionSummary _opiConditionSummary;

        private PavementWorkSummaryComputationHelper _pavementWorkSummaryComputationHelper;


        public PavementWorkSummary()
        {
            _costBudgetsWorkSummary = new CostBudgetsWorkSummary();
            _treatmentsWorkSummary = new TreatmentsWorkSummary();
            _iriConditionSummary = new IriConditionSummary();
            _opiConditionSummary = new OpiConditionSummary();
            _pavementWorkSummaryComputationHelper = new PavementWorkSummaryComputationHelper();
        }

        public ChartRowsModel Fill(
            ExcelWorksheet worksheet,
            SimulationOutput reportOutputData,
            List<int> simulationYears,
            WorkSummaryModel workSummaryModel,
            Dictionary<string, Budget> yearlyBudgetAmount,
            IReadOnlyCollection<SelectableTreatment> selectableTreatments)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            // Getting list of treatments. It will be used in several places throughout this excel TAB
            var simulationTreatments = new List<(string Name, DTOs.Enums.AssetCategory AssetType, TreatmentCategory Category)>();

            foreach (var item in selectableTreatments)
            {
                simulationTreatments.Add((item.Name, (DTOs.Enums.AssetCategory)item.AssetCategory, item.Category));
            }

            simulationTreatments.Sort((a, b) => a.Name.CompareTo(b.Name));


            var costAndLengthPerTreatmentPerYear = new Dictionary<int, Dictionary<string, (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>(); // Year, treatmentName, cost, length
            var costAndLengthPerTreatmentGroupPerYear = new Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>>();

            _pavementWorkSummaryComputationHelper.FillDataToUseInExcel(reportOutputData, costAndLengthPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear);
            var workTypeTotals = _pavementWorkSummaryComputationHelper.CalculateWorkTypeTotals(costAndLengthPerTreatmentPerYear, simulationTreatments);

            var chartRowsModel = _costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, simulationYears, yearlyBudgetAmount, costAndLengthPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear, simulationTreatments, workTypeTotals);
            chartRowsModel = _treatmentsWorkSummary.FillTreatmentsWorkSummarySections(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear, simulationTreatments, workTypeTotals, chartRowsModel);
            chartRowsModel = _iriConditionSummary.FillIriConditionSummarySection(worksheet, currentCell, reportOutputData, chartRowsModel);
            chartRowsModel = _opiConditionSummary.FillOpiConditionSummarySection(worksheet, currentCell, reportOutputData, chartRowsModel);

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();

            return chartRowsModel;
        }
    }
}
