using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentOptionDetails
    {
        public static TreatmentOptionDetail Detail()
        {
            var detail = new TreatmentOptionDetail(
                "Treatment", 100, 200, 3, 9);
            return detail;
        }
    }
}
