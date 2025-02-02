﻿using BridgeCare.Models;
using BridgeCare.Models.SummaryReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BridgeCare.Services
{
    public class BridgeDataHelper
    {
        public List<SimulationDataModel> GetSimulationDataModels(DataTable simulationDataTable, List<int> simulationYears, IQueryable<ReportProjectCost> projectCostModels, List<BudgetsPerBRKey> budgetsPerBrKey)
        {
            var simulationDataModels = new List<SimulationDataModel>();
            var projectCostsList = projectCostModels.ToList();
            foreach (DataRow simulationRow in simulationDataTable.Rows)
            {
                var bridgeDataPerSection = budgetsPerBrKey.Where(b => b.SECTIONID == Convert.ToUInt32(simulationRow["SECTIONID"])).ToList();
                var simulationDM = CreatePrevYearSimulationMdel(simulationRow);
                simulationDM.RiskScore = Convert.ToDouble(simulationRow["RISK_SCORE_" + 0]);
                var projectCostEntries = projectCostsList.Where(pc => pc.SECTIONID == Convert.ToUInt32(simulationRow["SECTIONID"])).ToList();
                AddAllYearsData(simulationRow, simulationYears, projectCostEntries, simulationDM, bridgeDataPerSection);
                simulationDataModels.Add(simulationDM);
            }

            return simulationDataModels;
        }        

        private SimulationDataModel CreatePrevYearSimulationMdel(DataRow simulationRow)
        {
            var blankBudgetForPreviousYear = new BudgetsPerBRKey { Budget = "", IsCommitted = false, Treatment = "" };
            YearsData yearsData = AddYearsData(simulationRow, null, 0, blankBudgetForPreviousYear);
            return new SimulationDataModel
            {
                YearsData = new List<YearsData>() { yearsData },
                SectionId = Convert.ToInt32(simulationRow["SECTIONID"])
            };
        }

        private void AddAllYearsData(DataRow simulationRow, List<int> simulationYears, List<ReportProjectCost> projectCostEntries, SimulationDataModel simulationDM, List<BudgetsPerBRKey> bridgeDataPerSection)
        {
            var yearsDataModels = new List<YearsData>();
            foreach (int year in simulationYears)
            {
                var budgetPerBrKey = new BudgetsPerBRKey() { Budget = "", IsCommitted = false, Treatment = ""};
                var projectCostEntry = projectCostEntries.Where(p => p.YEARS == year).FirstOrDefault();
                if(bridgeDataPerSection.Count > 0 && bridgeDataPerSection != null)
                {
                    budgetPerBrKey = bridgeDataPerSection.Where(p => p.YEARS == year).FirstOrDefault();
                }
                yearsDataModels.Add(AddYearsData(simulationRow, projectCostEntry, year, budgetPerBrKey));
            }
            simulationDM.YearsData.AddRange(yearsDataModels.OrderBy(y => y.Year).ToList());
        }

        private YearsData AddYearsData(DataRow simulationRow, ReportProjectCost projectCostEntry, int year, BudgetsPerBRKey budgetPerBrKey)
        {
            var yearsData = new YearsData
            {
                Deck = simulationRow["DECK_SEEDED_" + year].ToString(),
                Super = simulationRow["SUP_SEEDED_" + year].ToString(),
                Sub = simulationRow["SUB_SEEDED_" + year].ToString(),
                Culv = simulationRow["CULV_SEEDED_" + year].ToString(),
                DeckD = simulationRow["DECK_DURATION_N_" + year].ToString(),
                SuperD = simulationRow["SUP_DURATION_N_" + year].ToString(),
                SubD = simulationRow["SUB_DURATION_N_" + year].ToString(),
                CulvD = simulationRow["CULV_DURATION_N_" + year].ToString(),
                Year = year
            };
            var isDeckConverted = double.TryParse(yearsData.Deck, out var deck);
            var isCulvConverted = double.TryParse(yearsData.Culv, out var culv);
            var isSuperConverted = double.TryParse(yearsData.Super, out var super);
            var isSubConverted = double.TryParse(yearsData.Sub, out var sub);
            if(isDeckConverted && isCulvConverted && isSuperConverted && isSubConverted)
            {
                yearsData.MinC = Math.Min(deck, Math.Min(culv, Math.Min(super, sub)));
            }
            yearsData.SD = yearsData.MinC < 5 ? "Y" : "N";

            yearsData.Project = year != 0 ? projectCostEntry?.TREATMENT : string.Empty;

            double roundedCost = 0;
            if (projectCostEntry != null)
            {
                var amount = projectCostEntry.COST_;
                //if (amount >= 500)
                //{
                    roundedCost = amount % 1000 >= 500 ? amount + 1000 - amount % 1000 : amount - amount % 1000;
                //}
                //else
                //{
                //    roundedCost = 1000;
                //}
            }
            
            yearsData.Cost = year != 0 ? roundedCost : 0;
            yearsData.Project = yearsData.Project == null ? "No Treatment" : yearsData.Project;
            yearsData.Budget = budgetPerBrKey != null ? budgetPerBrKey.Budget : "";
            yearsData.ProjectPick = budgetPerBrKey != null ?
                (budgetPerBrKey.ProjectType == 0 ? "BAMs Pick" :
                (budgetPerBrKey.ProjectType == 1 ? "Committed Pick" :
                (budgetPerBrKey.ProjectType == 2 ? "Cash Flow" : "Scheduled")
                )) :
                "BAMs Pick";
            yearsData.ProjectPickType = budgetPerBrKey != null ? budgetPerBrKey.ProjectType : 0;
            yearsData.Treatment = budgetPerBrKey != null ? budgetPerBrKey.Treatment : "";
            return yearsData;
        }        
    }
}
