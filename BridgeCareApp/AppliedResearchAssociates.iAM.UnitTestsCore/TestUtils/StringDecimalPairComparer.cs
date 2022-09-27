using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class StringDecimalPairComparer
    {
        public static int Compare(StringDecimalPair pair1, StringDecimalPair pair2)
        {
            var compareStrings = pair1.String.CompareTo(pair2.String);
            if (compareStrings != 0)
            {
                return compareStrings;
            }
            return pair1.Decimal.CompareTo(pair2.Decimal);
        }
    }
}
