using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis;

public class AttributeValueHistoryProvider
{
    public IEnumerable<Attribute> HistoricalAttributes => _HistoryPerAttribute?.Keys ?? Enumerable.Empty<Attribute>();

    public virtual void ClearHistory()
    {
        _HistoryPerAttribute?.Clear();
        _HistoryPerAttribute = null;
    }

    public IAttributeValueHistory GetHistory(Attribute attribute) => attribute switch
    {
        Attribute<double> attribute_double => GetHistory(attribute_double),
        Attribute<string> attribute_string => GetHistory(attribute_string),
        _ => throw new NotImplementedException($"Non-generic value history is not yet implemented for attribute type {attribute.GetType().FullName}.")
    };

    public IAttributeValueHistory<T> GetHistory<T>(Attribute<T> attribute)
    {
        if (!HistoryPerAttribute.TryGetValue(attribute, out var history))
        {
            history = CreateHistory(attribute);
            HistoryPerAttribute.Add(attribute, history);
        }

        return (IAttributeValueHistory<T>)history;
    }

    protected virtual IAttributeValueHistory<T> CreateHistory<T>(Attribute<T> attribute) => new AttributeValueHistory<T>(attribute);

    private Dictionary<Attribute, object> _HistoryPerAttribute;

    private Dictionary<Attribute, object> HistoryPerAttribute => _HistoryPerAttribute ??= new();
}
