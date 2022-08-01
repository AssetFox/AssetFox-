using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public abstract class AttributeConnection
    {
        public Attribute Attribute { get; }

        public BaseDataSourceDTO DataSource { get; }

        public abstract IEnumerable<IAttributeDatum> GetData<T>();

        protected AttributeConnection(Attribute attribute, BaseDataSourceDTO dataSource)
        {
            Attribute = attribute;
            DataSource = dataSource;
        }
    }
}
