using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class AttributeDatum<T> : IAttributeDatum
    {
        public Location Location { get; }

        public Attribute Attribute { get; }

        public T Value { get; }

        public DateTime TimeStamp { get; }

        public AttributeDatum(Attribute attribute, T value, Location location, DateTime timeStamp)
        {
            Attribute = attribute;
            Value = value;
            Location = location;
            TimeStamp = timeStamp;
        }
    }
}
