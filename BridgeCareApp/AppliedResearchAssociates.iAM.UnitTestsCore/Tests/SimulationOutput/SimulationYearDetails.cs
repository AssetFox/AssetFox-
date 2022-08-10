using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationYearDetails
    {
        public static SimulationYearDetail YearDetail(
            int year,
            SimulationOutputSetupContext context)
        {
            var yearDetail = new SimulationYearDetail(year)
            {
                ConditionOfNetwork = 23,
            };
            var assetDetail = AssetDetails.AssetDetail(context, context.ManagedAssetName);
            var budgetDetail = BudgetDetails.Detail();
            var deficientConditionGoalDetail = DeficientConditionGoalDetails.Detail(context);
            var targetConditionGoalDetail = TargetConditionGoalDetails.Detail(context);
            yearDetail.Assets.Add(assetDetail);
            yearDetail.Budgets.Add(budgetDetail);
            yearDetail.DeficientConditionGoals.Add(deficientConditionGoalDetail);
            yearDetail.TargetConditionGoals.Add(targetConditionGoalDetail);
            return yearDetail;
        }
    }
}
