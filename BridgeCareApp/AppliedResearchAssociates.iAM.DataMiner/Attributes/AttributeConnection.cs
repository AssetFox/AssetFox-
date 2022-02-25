using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class AttributeConnection
    {
        public Attribute Attribute { get; }

        public abstract IEnumerable<IAttributeDatum> GetData<T>();

        public const string DateColumnName = "DATE_";
        public const string DataColumnName = "DATA_";
        public const string LocationIdentifierString = "LOCATION_IDENTIFIER";

        protected AttributeConnection(Attribute attribute) => Attribute = attribute;
    }
}
