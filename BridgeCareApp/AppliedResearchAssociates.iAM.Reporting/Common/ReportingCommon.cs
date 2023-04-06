using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Reporting.Common
{
    public static class ReportCommon
    {
        public enum NumberSuffix
        {
            P, // Value is less than one thousand
            K, // Thousand
            M, // Million
            B, // Billion
            T, // Trillion
            Q  // Quadrillion
        }

        public static string FormatNumber(decimal numberToFormat, int decimalPlaces = 0)
        {
            // Get the default string representation of the numberToFormat.
            var numberString = numberToFormat.ToString();

            //check each suffix until found
            foreach (NumberSuffix suffix in Enum.GetValues<NumberSuffix>())
            {
                // Assign the amount of digits to base 10.
                var currentValue = Convert.ToDecimal(1 * Math.Pow(10, (int)suffix * 3));

                // Get the suffix value.
                var suffixValue = Enum.GetName(typeof(NumberSuffix), (int)suffix);

                // If the suffix is the placeholder, set it to an empty string.
                if ((int)suffix == 0) { suffixValue = string.Empty; }

                // Set the return value to a rounded value with the suffix.
                if (numberToFormat >= currentValue)
                {
                    numberString = $"{Math.Round(numberToFormat / currentValue, decimalPlaces, MidpointRounding.ToZero)}{suffixValue}";
                }
                else { break; }
            }

            //return formatted string
            return numberString;
        }
    }    
}
