using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class AttributeDatum<T> : IAttributeDatum
    {
        public Guid Id { get; }
        public Location Location { get; }

        public Attribute Attribute { get; }

        public T Value { get; }

        public DateTime TimeStamp { get; }

        public AttributeDatum(Guid id, Attribute attribute, T value, Location location, DateTime timeStamp)
        {
            Id = id;
            Attribute = attribute;
            Value = value;
            Location = location;
            TimeStamp = timeStamp;
        }
    }
}
