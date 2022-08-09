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
            return yearDetail;
        }
    }
}
