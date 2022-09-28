using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppliedResearchAssociates.iAM.Common.PerformanceMeasurement
{
    public class EventMemoModel
    {
        public string Text { get; set; }
        public DateTime UtcTime { get; set; }
    }

    public static class EventMemoModelListExtensions
    {
        /// <summary>The returned string shows the elapsed time between the current and previous memos. If there
        /// is no previous memo, the empty string is returned.</summary> 
        public static string Mark(this List<EventMemoModel> eventList, string text)
        {
            var memo = EventMemoModels.Now(text);
            eventList.Add(memo);
            if (eventList.Count > 1)
            {
                var previous = eventList[eventList.Count - 2];
                var elapsed = (memo.UtcTime - previous.UtcTime).TotalMilliseconds;
                var elapsedString = $"{(int)elapsed}ms {memo.Text} ";
                return elapsedString;
            }
            return "";
        }

        public static string MarkInformation(this List<EventMemoModel> eventList, string text, ILog logger)
        {
            var memo = eventList.Mark(text);
            logger.Information(memo);
            return memo;
        }

        private static string FormatMilliseconds(int milliseconds)
        {
            return milliseconds.ToString("N0");
        }

        private static string FormatMilliseconds(double milliseconds)
        {
            return FormatMilliseconds((int)milliseconds);
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
                        var elapsedString = $"{FormatMilliseconds(elapsed)} {memo.Text} ";
                        builder.Append(elapsedString);
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
                    var roundedTime = FormatMilliseconds(elapsed);
                    var maxKeyLength = eventList.Select(x => x.Text.Length).Max();
                    builder.AppendLine($"{memo.Text.PadRight(maxKeyLength + 1)}{roundedTime}");
                }
                previous = memo.UtcTime;
            }
            if (includeLineForTotal && eventList.Any())
            {
                var total = eventList.Last().UtcTime - eventList.First().UtcTime;
                var totalMs = FormatMilliseconds(total.TotalMilliseconds);
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
