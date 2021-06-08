using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public static class DoubleWarningStrings
    {
        public static string InfinityOrNanWarning(double x)
            => double.IsNaN(x) ? "'not a number'" :
                    double.IsPositiveInfinity(x) ? "infinity" : "negative infinity";
    }
}
