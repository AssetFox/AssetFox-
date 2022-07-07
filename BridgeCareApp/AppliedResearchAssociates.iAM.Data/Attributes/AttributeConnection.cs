using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public abstract class AttributeConnection
    {
        public Attribute Attribute { get; }

        public abstract IEnumerable<IAttributeDatum> GetData<T>();   

        protected AttributeConnection(Attribute attribute) => Attribute = attribute;
    }
}
