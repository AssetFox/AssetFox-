using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class AttributeValueHistory<T> : IAttributeValueHistory<T>
{
    public T MostRecentValue => MostRecentYear is int year ? ValuePerYear[year] : Attribute.DefaultValue;

    public int? MostRecentYear => Years.AsNullables().Max();

    public IEnumerable<int> Years => ValuePerYear.Keys;

    public T this[int year]
    {
        get => ValuePerYear[year];
        set => ValuePerYear[year] = value;
    }

    public void Add(int year, T value) => ValuePerYear.Add(year, value);

    public void Add(KeyValuePair<int, T> yearValue) => Add(yearValue.Key, yearValue.Value);

    public IEnumerator<KeyValuePair<int, T>> GetEnumerator() => ValuePerYear.GetEnumerator();

    public bool TryGetValue(int year, out T value) => ValuePerYear.TryGetValue(year, out value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal AttributeValueHistory(Attribute<T> attribute) => Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

    private readonly Attribute<T> Attribute;
    private readonly Dictionary<int, T> ValuePerYear = new();
}
