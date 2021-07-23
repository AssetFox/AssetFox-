
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using BridgeCareCore.Services;
using BridgeCareCore.Services.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeData;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummary;
using BridgeCareCore.Services.SummaryReport.BridgeWorkSummaryByBudget;
using BridgeCareCore.Services.SummaryReport.GraphTabs;
using BridgeCareCore.Services.SummaryReport.GraphTabs.BPN;
using BridgeCareCore.Services.SummaryReport.GraphTabs.NHSConditionCharts;
using BridgeCareCore.Services.SummaryReport.Parameters;
using BridgeCareCore.Services.SummaryReport.ShortNameGlossary;
using BridgeCareCore.Services.SummaryReport.UnfundedTreatmentCommon;
using BridgeCareCore.Services.SummaryReport.UnfundedTreatmentFinalList;
using BridgeCareCore.Services.SummaryReport.UnfundedTreatmentTime;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeCareCore.StartupExtension
{
    public static class SummaryReportExtension
    {
        public static void AddSummaryReportDataTABs(this IServiceCollection services)
        {
            services.AddScoped<ISummaryReportGenerator, SummaryReportGenerator>();
            services.AddScoped<ISummaryReportHelper, SummaryReportHelper>();
            services.AddScoped<IHighlightWorkDoneCells, HighlightWorkDoneCells>();
            services.AddScoped<IUnfundedTreatmentCommon, UnfundedTreatmentCommon>();
            services.AddScoped<IUnfundedTreatmentFinalList, UnfundedTreatmentFinalList>();
            services.AddScoped<IUnfundedTreatmentTime, UnfundedTreatmentTime>();
            services.AddScoped<IBridgeWorkSummary, BridgeWorkSummary>();
            services.AddScoped<IBridgeWorkSummaryByBudget, BridgeWorkSummaryByBudget>();
            services.AddScoped<SummaryReportGlossary>();
            services.AddScoped<SummaryReportParameters>();
            services.AddScoped<CostBudgetsWorkSummary>();
            services.AddScoped<BridgesCulvertsWorkSummary>();
            services.AddScoped<BridgeRateDeckAreaWorkSummary>();
            services.AddScoped<NHSBridgeDeckAreaWorkSummary>();
            services.AddScoped<DeckAreaBridgeWorkSummary>();
            services.AddScoped<PostedClosedBridgeWorkSummary>();
            services.AddScoped<BridgeWorkSummaryCommon>();
            services.AddScoped<BridgeWorkSummaryComputationHelper>();
            services.AddScoped<CulvertCost>();
            services.AddScoped<BridgeWorkCost>();
            services.AddScoped<CommittedProjectCost>();
            services.AddScoped<ICommittedProjectService, CommittedProjectService>();
            services.AddScoped<WorkSummaryModel>();

            services.AddScoped<ProjectsCompletedCount>();
        }

        public static void AddSummaryReportGraphTABs(this IServiceCollection services)
        {
            services.AddScoped<IAddGraphsInTabs, AddGraphsInTabs>();
            services.AddScoped<GraphData>(); // Not a graph tab, but some graph tabs are dependent on this
            services.AddScoped<ConditionPercentageChart>();
            services.AddScoped<PoorBridgeCount>();
            services.AddScoped<PoorBridgeDeckArea>();
            services.AddScoped<StackedColumnChartCommon>();

            services.AddScoped<CombinedPostedAndClosed>();
            services.AddScoped<CashNeededByBPN>();
            services.AddScoped<IAddBPNGraphTab, AddBPNGraphTab>();

            services.AddScoped<BPNAreaChart>();
            services.AddScoped<BPNCountChart>();
        }
    }
}
