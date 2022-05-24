using System;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public interface IAttributeDatum
    {
        Location Location { get; }

        Attribute Attribute { get; }

        DateTime TimeStamp { get; }
    }
}
