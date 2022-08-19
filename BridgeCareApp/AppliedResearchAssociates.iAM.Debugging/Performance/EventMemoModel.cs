using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppliedResearchAssociates.iAM.Debugging
{
    public class EventMemoModel
    {
        public string Text { get; set; }
        public DateTime UtcTime { get; set; }
    }

    public static class EventMemoModelListExtensions
    {
        public static void Mark(this List<EventMemoModel> eventList, string text)
        {
            var memo = EventMemoModels.Now(text);
            eventList.Add(memo);
        }

        public static string ToSingleLineString(this List<EventMemoModel> eventList)
        {
            var builder = new StringBuilder();
            bool first = true;
            DateTime previous = DateTime.MinValue;
            if (eventList.Any())
            {
                foreach (var memo in eventList)
                {
                    if (!first)
                    {
                        var elapsed = (memo.UtcTime - previous).TotalMilliseconds;
                        builder.Append($"{(int)elapsed} {memo.Text} ");
                    }
                    previous = memo.UtcTime;
                    first = false;
                }
            }
            return builder.ToString();
        }

        public static string ToMultilineString(this List<EventMemoModel> eventList, bool includeLineForTotal = false)
        {
            var builder = new StringBuilder();
            DateTime previous = DateTime.MinValue;
            foreach (var memo in eventList)
            {
                if (previous != DateTime.MinValue)
                {
                    var elapsed = (memo.UtcTime - previous).TotalMilliseconds;
                    var roundedTime = (int)elapsed;
                    var maxKeyLength = eventList.Select(x => x.Text.Length).Max();
                    builder.AppendLine($"{memo.Text.PadRight(maxKeyLength + 1)}{roundedTime}");
                }
                previous = memo.UtcTime;
            }
            if (includeLineForTotal && eventList.Any())
            {
                var total = eventList.Last().UtcTime - eventList.First().UtcTime;
                var totalMs = (int)total.TotalMilliseconds;
                builder.AppendLine($"Total: {totalMs}");
            }
            return builder.ToString();
        }

        public static string ToGroupedString(this List<EventMemoModel> eventList)
        {
            Dictionary<string, double> TimeDictionary = new Dictionary<string, double>();
            EventMemoModel previousMemo = null;
            var allKeys = new List<string>();
            foreach (var memo in eventList)
            {
                if (previousMemo != null)
                {
                    var dt = (memo.UtcTime - previousMemo.UtcTime).TotalMilliseconds;
                    if (TimeDictionary.ContainsKey(memo.Text))
                    {
                        TimeDictionary[memo.Text] += dt;
                    }
                    else
                    {
                        TimeDictionary[memo.Text] = dt;
                        allKeys.Add(memo.Text);
                    }
                }
                previousMemo = memo;
            }
            var keys = TimeDictionary.Keys;
            var builder = new StringBuilder();
            if (keys.Any())
            {
                var maxKeyLength = allKeys.Max(k => k.Length);
                foreach (var key in allKeys)
                {
                    var roundedValue = (int)TimeDictionary[key];
                    builder.AppendLine($"{key.PadRight(maxKeyLength + 1)} {roundedValue,6}");
                }
            }
            return builder.ToString();
        }
    }
}
