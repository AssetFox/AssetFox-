using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class StringDecimalPairs
    {
        public static StringDecimalPair StringOnly(string str)
        {
            var pair = new StringDecimalPair
            {
                String = str,
                Decimal = decimal.MinValue,
            };
            return pair;
        }

        internal static StringDecimalPair StringAndDecimal(
            string prefixString, decimal parsedDecimal)
        {
            var pair = new StringDecimalPair
            {
                String = prefixString,
                Decimal = parsedDecimal,
            };
            return pair;
        }
    }
}
