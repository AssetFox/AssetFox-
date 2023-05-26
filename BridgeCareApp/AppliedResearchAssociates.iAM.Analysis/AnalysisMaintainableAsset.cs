using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class AnalysisMaintainableAsset : WeakEntity, IValidator
{
    public static Func<AttributeValueHistoryProvider> GetHistoryProvider { get; set; } = () => new AttributeValueHistoryProvider();

    public string AssetName { get; set; }

    public IEnumerable<Attribute> HistoricalAttributes => HistoryProvider.HistoricalAttributes;

    public AttributeValueHistoryProvider HistoryProvider { get; }

    public Network Network { get; }

    public string ShortDescription => AssetName;

    public Equation SpatialWeighting { get; }

    public ValidatorBag Subvalidators => new() { SpatialWeighting };

    public ValidationResultBag GetDirectValidationResults()
    {
        var results = new ValidationResultBag();

        if (string.IsNullOrWhiteSpace(AssetName))
        {
            results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(AssetName));
        }

        return results;
    }

    public IAttributeValueHistory GetHistory(Attribute attribute) => HistoryProvider.GetHistory(attribute);

    public IAttributeValueHistory<T> GetHistory<T>(Attribute<T> attribute) => HistoryProvider.GetHistory(attribute);

    internal AnalysisMaintainableAsset(Network network)
    {
        Network = network ?? throw new ArgumentNullException(nameof(network));
        SpatialWeighting = new Equation(Network.Explorer);

        HistoryProvider = GetHistoryProvider();
    }
}
