using System;
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
            CalculatedNumber.CopyFrom(original.CalculatedNumber);
            FixedNumber.CopyFrom(original.FixedNumber);
            Text.CopyFrom(original.Text);
        }

        public IReadOnlyCollection<string> NumberKeys => FixedNumber.Keys;

        public IReadOnlyCollection<string> TextKeys => Text.Keys;

        public virtual double GetNumber(string key) => FixedNumber.TryGetValue(key, out var value) ? value : CalculatedNumber[key]();

        public virtual string GetText(string key) => Text[key];

        public virtual void SetNumber(string key, double value)
        {
            FixedNumber[key] = value;
            _ = CalculatedNumber.Remove(key);
        }

        public virtual void SetNumber(string key, Func<double> getValue)
        {
            CalculatedNumber[key] = getValue ?? throw new ArgumentNullException(nameof(getValue));
            _ = FixedNumber.Remove(key);
        }

        public virtual void SetText(string key, string value) => Text[key] = value ?? "";

        private readonly Dictionary<string, Func<double>> CalculatedNumber = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, double> FixedNumber = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> Text = new(StringComparer.OrdinalIgnoreCase);
    }
}
