using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Attribute
    {
        public Attribute(string name, AttributeConnection connection)
        {
            Guid = Guid.NewGuid();
            Name = name;
            Connection = connection;
        }

        public Guid Guid { get; }

        public string Name { get; }

        public AttributeConnection Connection { get; }
    }
}
