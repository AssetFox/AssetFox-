﻿using System;
using System.Collections.Generic;
using BridgeCare.Interfaces;
using BridgeCare.Interfaces.SummaryReport;
using BridgeCare.Models;
using BridgeCare.Services.SummaryReport.WorkSummary;
using OfficeOpenXml;

namespace BridgeCare.Services
{
    public class BridgeWorkSummary
    {
        private readonly CostBudgetsWorkSummary costBudgetsWorkSummary;
        private readonly BridgesCulvertsWorkSummary bridgesCulvertsWorkSummary;
        private readonly BridgeRateDeckAreaWorkSummary bridgeRateDeckAreaWorkSummary;
        private readonly IBridgeWorkSummaryDataRepository _bridgeWorkSummaryDataRepository;
        private readonly NHSBridgeDeckAreaWorkSummary nhsBridgeDeckAreaWorkSummary;
        private readonly PostedClosedBridgeWorkSummary postedClosedBridgeWorkSummary;
        private readonly DeckAreaBridgeWorkSummary deckAreaBridgeWorkSummary;
        private readonly IWorkSummaryByBudgetRepository _workSummaryByBudgetRepository;
        private readonly IBridgeDataRepository _bridgeDataRepository;

        public BridgeWorkSummary(CostBudgetsWorkSummary costBudgetsWorkSummary, BridgesCulvertsWorkSummary bridgesCulvertsWorkSummary,
            BridgeRateDeckAreaWorkSummary bridgeRateDeckAreaWorkSummary, IBridgeWorkSummaryDataRepository bridgeWorkSummaryDataRepository,
            NHSBridgeDeckAreaWorkSummary nhsBridgeDeckAreaWorkSummary, PostedClosedBridgeWorkSummary postedClosedBridgeWorkSummary,
            DeckAreaBridgeWorkSummary deckAreaBridgeWorkSummary, IWorkSummaryByBudgetRepository workSummaryByBudgetRepository, IBridgeDataRepository bridgeDataRepository)
        {
            this.costBudgetsWorkSummary = costBudgetsWorkSummary ?? throw new ArgumentNullException(nameof(costBudgetsWorkSummary));
            this.bridgesCulvertsWorkSummary = bridgesCulvertsWorkSummary ?? throw new ArgumentNullException(nameof(bridgesCulvertsWorkSummary));
            this.bridgeRateDeckAreaWorkSummary = bridgeRateDeckAreaWorkSummary ?? throw new ArgumentNullException(nameof(bridgeRateDeckAreaWorkSummary));
            this._bridgeWorkSummaryDataRepository = bridgeWorkSummaryDataRepository ?? throw new ArgumentNullException(nameof(bridgeWorkSummaryDataRepository));
            this.nhsBridgeDeckAreaWorkSummary = nhsBridgeDeckAreaWorkSummary ?? throw new ArgumentNullException(nameof(nhsBridgeDeckAreaWorkSummary));
            this.postedClosedBridgeWorkSummary = postedClosedBridgeWorkSummary ?? throw new ArgumentNullException(nameof(postedClosedBridgeWorkSummary));
            this.deckAreaBridgeWorkSummary = deckAreaBridgeWorkSummary ?? throw new ArgumentNullException(nameof(deckAreaBridgeWorkSummary));
            _workSummaryByBudgetRepository = workSummaryByBudgetRepository ?? throw new ArgumentNullException(nameof(workSummaryByBudgetRepository));
            this._bridgeDataRepository = bridgeDataRepository ?? throw new ArgumentNullException(nameof(bridgeDataRepository));
        }

        /// <summary>
        /// Fill Work Summary report
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="simulationDataModels"></param>
        /// <param name="bridgeDataModels"></param>
        /// <param name="simulationYears"></param>
        /// <param name="dbContext"></param>
        /// <param name="simulationId"></param>        
        /// <returns>ChartRowsModel object for usage in other tab reports.</returns>
        public ChartRowsModel Fill(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<BridgeDataModel> bridgeDataModels, List<int> simulationYears, BridgeCareContext dbContext, SimulationModel simulationModel, List<string> treatments)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };
            var yearlyBudgetAmounts = _bridgeWorkSummaryDataRepository.GetYearlyBudgetAmounts(simulationModel.simulationId, simulationYears, dbContext);

            var comittedProjectsData = _workSummaryByBudgetRepository.GetAllCommittedProjects(simulationModel, dbContext);

            costBudgetsWorkSummary.FillCostBudgetWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels, yearlyBudgetAmounts, treatments, comittedProjectsData);

            bridgesCulvertsWorkSummary.FillBridgesCulvertsWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels, treatments);

            var chartRowsModel = bridgeRateDeckAreaWorkSummary.FillBridgeRateDeckAreaWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels);

            nhsBridgeDeckAreaWorkSummary.FillNHSBridgeDeckAreaWorkSummarySections(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);

            chartRowsModel = deckAreaBridgeWorkSummary.FillPoorDeckArea(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);

            //chartRowsModel = postedClosedBridgeWorkSummary.FillPostedBridgeCount(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);

            //chartRowsModel = deckAreaBridgeWorkSummary.FillPostedDeckArea(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);

            //chartRowsModel = postedClosedBridgeWorkSummary.FillClosedBridgeCount(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);

            //chartRowsModel = deckAreaBridgeWorkSummary.FillClosedDeckArea(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);

            //chartRowsModel = postedClosedBridgeWorkSummary.FillBridgeCountTotal(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);

            chartRowsModel = postedClosedBridgeWorkSummary.FillMoneyNeededByBPN(worksheet, currentCell, simulationYears, simulationDataModels, bridgeDataModels, chartRowsModel);

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();
            return chartRowsModel;
        }        
    }
}
