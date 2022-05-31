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
