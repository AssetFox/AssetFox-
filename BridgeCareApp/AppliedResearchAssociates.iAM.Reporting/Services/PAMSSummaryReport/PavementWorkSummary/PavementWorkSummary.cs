using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.Reporting.Models;
using CurrentCell = AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport.CurrentCell;

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
            Dictionary<string, Budget> yearlyBudgetAmount,
            IReadOnlyCollection<SelectableTreatment> selectableTreatments,
            ICollection<CommittedProject> committedProjects,
            Dictionary<string, string> treatmentCategoryLookup,
            List<BaseCommittedProjectDTO> committedProjectsForWorkOutsideScope,
            bool shouldBundleFeasibleTreatments)
        {
            var currentCell = new CurrentCell { Row = 10, Column = 1 };
            var yearlyCostCommittedProj = new Dictionary<int, Dictionary<string, List<CommittedProjectMetaData>>>();
            // Getting list of treatments. It will be used in several places throughout this excel TAB
            var simulationTreatments = new List<(string Name, string AssetType, TreatmentCategory Category)>();

            foreach (var item in selectableTreatments)
            {
                var category = SummaryReportHelper.GetCategory(item.Category);
                simulationTreatments.Add((item.Name, item.AssetCategory, category));
            }

            simulationTreatments.Sort((a, b) => a.Name.CompareTo(b.Name));

            // Year, treatmentName, surfaceId, cost, length, 
            var costLengthPerSurfaceIdPerTreatmentPerYear = new Dictionary<int, // year
                            Dictionary<string, // treatmentName
                            Dictionary<int, // surfaceId
                            (decimal treatmentCost, decimal compositeTreatmentCost, int length)>>>();
            var costAndLengthPerTreatmentGroupPerYear = new Dictionary<int, Dictionary<PavementTreatmentHelper.TreatmentGroup, (decimal treatmentCost, int length)>>();
            
            _pavementWorkSummaryComputationHelper.FillDataToUseInExcel(reportOutputData, yearlyCostCommittedProj, costLengthPerSurfaceIdPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear, treatmentCategoryLookup, committedProjectsForWorkOutsideScope, simulationTreatments, shouldBundleFeasibleTreatments);
            var workTypeTotals = _pavementWorkSummaryComputationHelper.CalculateWorkTypeTotals(costLengthPerSurfaceIdPerTreatmentPerYear, simulationTreatments);
            var chartRowsModel = _costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, simulationYears, yearlyBudgetAmount, costLengthPerSurfaceIdPerTreatmentPerYear, yearlyCostCommittedProj, costAndLengthPerTreatmentGroupPerYear, simulationTreatments, workTypeTotals, committedProjects, committedProjectsForWorkOutsideScope, shouldBundleFeasibleTreatments);
            chartRowsModel = _treatmentsWorkSummary.FillTreatmentsWorkSummarySections(worksheet, currentCell, simulationYears, costLengthPerSurfaceIdPerTreatmentPerYear, costAndLengthPerTreatmentGroupPerYear, simulationTreatments, workTypeTotals, chartRowsModel, shouldBundleFeasibleTreatments);
            chartRowsModel = _iriConditionSummary.FillIriConditionSummarySection(worksheet, currentCell, reportOutputData, chartRowsModel);
            chartRowsModel = _opiConditionSummary.FillOpiConditionSummarySection(worksheet, currentCell, reportOutputData, chartRowsModel);

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();

            return chartRowsModel;
        }        
    }
}
