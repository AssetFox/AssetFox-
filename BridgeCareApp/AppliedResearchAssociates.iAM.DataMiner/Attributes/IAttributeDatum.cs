using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public interface IAttributeDatum
    {
        Location Location { get; }

        Attribute Attribute { get; }

        DateTime TimeStamp { get; }
    }
}
