using System;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class RemainingLifeCalculator
{
    public RemainingLifeCalculator(AssetContext context, IGrouping<INumericAttribute, double> limits)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Attribute = limits.Key;
        SetCurrentValue();

        if (Attribute.IsDecreasingWithDeterioration)
        {
            Limit = limits.Max();
            _CurrentValueIsBeyondLimit = CurrentValueIsLessThanLimit;
        }
        else
        {
            Limit = limits.Min();
            _CurrentValueIsBeyondLimit = CurrentValueIsGreaterThanLimit;
        }
    }

    public bool CurrentValueIsBeyondLimit => _CurrentValueIsBeyondLimit();

    public double GetLimitLocationRelativeToLatestValues() => (Limit - PreviousValue.Value) / (CurrentValue - PreviousValue.Value);

    public void UpdateValue()
    {
        PreviousValue = CurrentValue;
        SetCurrentValue();
    }

    public sealed class Factory
    {
        public Factory(IGrouping<INumericAttribute, double> limits) => Limits = limits ?? throw new ArgumentNullException(nameof(limits));

        public RemainingLifeCalculator Create(AssetContext context) => new RemainingLifeCalculator(context, Limits);

        private readonly IGrouping<INumericAttribute, double> Limits;
    }

    private readonly Func<bool> _CurrentValueIsBeyondLimit;

    private readonly INumericAttribute Attribute;

    private readonly AssetContext Context;

    private readonly double Limit;

    private double CurrentValue;

    private double? PreviousValue;

    private bool CurrentValueIsGreaterThanLimit() => CurrentValue > Limit;

    private bool CurrentValueIsLessThanLimit() => CurrentValue < Limit;

    private void SetCurrentValue() => CurrentValue = Context.GetNumber(Attribute.Name);
}
