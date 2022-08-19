using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class StringDecimalRoundoff
    {
        /// <summary>Return value:
        /// -1 if our string does not end with a decimal number.
        /// </summary> 
        private static int IndexOfStartOfTrailingDecimal(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return -1;
            }
            bool foundDecimalPoint = false;
            bool foundDigit = false;
            for (int index = str.Length - 1; index >= 0; index--)
            {
                if (str[index] == '.')
                {
                    if (foundDecimalPoint)
                    {
                        return -1;
                    }
                    else
                    {
                        foundDecimalPoint = true;
                        continue;
                    }
                }
                if (str[index] >= '0' && str[index] <= '9')
                {
                    foundDigit = true;
                    continue;
                }
                if (foundDecimalPoint && foundDigit)
                {
                    return index + 1;
                }
                return -1;
            }
            return 0;
        }

        public static string RoundoffTrailingDecimal(string str, int decimalPlaces)
        {
            var decimalIndex = IndexOfStartOfTrailingDecimal(str);
            if (decimalIndex == -1 || decimalIndex == str.Length)
            {
                return str;
            }
            var trailingDecimal = str.Substring(decimalIndex);
            var parsedDecimal = decimal.Parse(trailingDecimal);
            var roundedDecimal = decimal.Round(parsedDecimal, decimalPlaces);
            var formatString = $"F{decimalPlaces}";
            var decimalString = roundedDecimal.ToString(formatString);
            str = string.Concat(str.Substring(0, decimalIndex), decimalString);
            return str;
        }
    }

    public class StringDecimalRoundoffTests
    {
        [Theory]
        [InlineData("hello: 1.234", "hello: 1.23")]
        [InlineData("hello: 1.235", "hello: 1.24")]
        [InlineData("hello: 123.000", "hello: 123.00")]
        [InlineData("hello: 234.", "hello: 234.00")]
        [InlineData("Banana", "Banana")]
        public void RoundoffString_Expected(string input, string expectedOutput)
        {
            var output = StringDecimalRoundoff.RoundoffTrailingDecimal(input, 2);
            Assert.Equal(expectedOutput, output);
        }
    }
}
