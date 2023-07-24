using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class Benefit : WeakEntity, IValidator
{
    internal Benefit()
    {
    }

    public INumericAttribute Attribute
    {
        get => _Attribute;
        set
        {
            _Attribute = value;

            if (Attribute == null)
            {
                _LimitValue = null;
            }
            else if (Attribute.IsDecreasingWithDeterioration)
            {
                _LimitValue = LimitDecreasingValue;
            }
            else
            {
                _LimitValue = LimitIncreasingValue;
            }
        }
    }

    public double Limit { get; set; }

    public ValidatorBag Subvalidators => new ValidatorBag();

    public string ShortDescription => nameof(Benefit);

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (Attribute == null)
        {
            results.Add(ValidationStatus.Error, "Attribute is unset.", this, nameof(Attribute));
        }

        return results;
    }

    public double GetValueRelativeToLimit(double benefit) => Math.Max(0, _LimitValue(benefit));

    internal double GetNetworkCondition(IEnumerable<AssetContext> network)
    {
        var networkSpatialWeight = network.Sum(context => context.GetSpatialWeight());
        var networkCondition = network.Sum(context => GetValueRelativeToLimit(context.GetNumber(Attribute.Name)) * context.GetSpatialWeight()) / networkSpatialWeight;
        return networkCondition;
    }

    private INumericAttribute _Attribute;

    private Func<double, double> _LimitValue;

    private double LimitDecreasingValue(double value) => value - Limit;

    private double LimitIncreasingValue(double value) => Limit - value;
}
