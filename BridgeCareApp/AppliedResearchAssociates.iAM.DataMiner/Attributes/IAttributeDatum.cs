using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public interface IAttributeDatum
    {
        Location Location { get; }

        Attribute Attribute { get; }

        DateTime TimeStamp { get; }
    }
}
