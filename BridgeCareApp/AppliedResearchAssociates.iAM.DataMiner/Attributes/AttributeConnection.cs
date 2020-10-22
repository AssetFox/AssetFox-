using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class AttributeConnection
    {
        public Attribute Attribute { get; }

        public abstract IEnumerable<IAttributeDatum> GetData<T>();

        public abstract event EventHandler<InformationEventArgs> Information;

        public AttributeConnection(Attribute attribute)
        {
            Attribute = attribute;
        }
    }
}
