﻿using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculateEvaluateScope
    {
        public CalculateEvaluateScope()
        {
        }

        public CalculateEvaluateScope(CalculateEvaluateScope original)
        {
            Number.CopyFrom(original.Number);
            Text.CopyFrom(original.Text);
            Timestamp.CopyFrom(original.Timestamp);
        }

        public ICollection<string> NumberKeys => Number.Keys;

        public ICollection<string> TextKeys => Text.Keys;

        public ICollection<string> TimestampKeys => Timestamp.Keys;

        public virtual double GetNumber(string key) => Number[key]();

        public virtual string GetText(string key) => Text[key];

        public virtual DateTime GetTimestamp(string key) => Timestamp[key];

        public virtual void SetNumber(string key, double value) => Number[key] = () => value;

        public virtual void SetNumber(string key, Func<double> getValue) => Number[key] = getValue;

        public virtual void SetText(string key, string value) => Text[key] = value ?? "";

        public virtual void SetTimestamp(string key, DateTime value) => Timestamp[key] = value;

        private readonly IDictionary<string, Func<double>> Number = new Dictionary<string, Func<double>>(StringComparer.OrdinalIgnoreCase);

        private readonly IDictionary<string, string> Text = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly IDictionary<string, DateTime> Timestamp = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
    }
}
