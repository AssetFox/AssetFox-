using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.Helpers
{
    public static class DoubleParseHelper
    {
        public static double TryParseDouble(string input, double defaultValue)
        {
            if (double.TryParse(input, out double value)) {
                return value;
            }
            return defaultValue;
        }

        public static double? TryParseNullableDouble(object input)
        {
            if (input is double value)
            {
                return (double)value;
            }
            if (input is int intValue)
            {
                return (double)intValue;
            }
            if (input == null)
            {
                return null;
            }
            var stringValue = input.ToString();
            return TryParseNullableDouble(stringValue);
        }

        public static double? TryParseNullableDouble(string input)
        {
            if (double.TryParse(input, out var value))
            {
                return value;
            }
            return null;
        }
    }
}
