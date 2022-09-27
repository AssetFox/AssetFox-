using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentSchedulingCollisionDetails
    {
        public static TreatmentSchedulingCollisionDetail Detail(int year)
        {
            var detail = new TreatmentSchedulingCollisionDetail(year, "Treatment");
            return detail;
        }
    }
}
