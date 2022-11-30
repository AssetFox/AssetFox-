using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DictionaryDebugConvenience
    {
        public static string ToDebugString<T, U>(Dictionary<T, U> dictionary)
        {
            var maxKeyLength = dictionary.Keys.Select(x => x.ToString().Length).DefaultIfEmpty(0).Max();
            var lines = dictionary.Select(x =>
            $"{x.Key.ToString().PadRight(maxKeyLength)} {x.Value}").ToList();
            var returnValue = string.Join(Environment.NewLine, lines.OrderBy(x => x));
            return returnValue;
        }
    }
}
