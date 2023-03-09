using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentRejectionDetails
    {
        public static TreatmentRejectionDetail Detail()
        {
            var detail = new TreatmentRejectionDetail(
                "Treatment",
                TreatmentRejectionReason.Undefined,
                default);
            return detail;
        }
    }
}
