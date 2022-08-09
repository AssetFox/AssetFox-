using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationOutputs
    {
        public static SimulationOutput SimulationOutput(
            SimulationOutputSetupContext context
            )
        {
            var output = new SimulationOutput
            {
                InitialConditionOfNetwork = 77,
            };
            var detail = AssetSummaryDetails.Detail(context);
            output.InitialAssetSummaries.Add(detail);
            foreach (var year in context.Years)
            {
                var yearDetail = SimulationYearDetails.YearDetail(year, context);
                output.Years.Add(yearDetail);
            }
            return output;
        }
    }
}
