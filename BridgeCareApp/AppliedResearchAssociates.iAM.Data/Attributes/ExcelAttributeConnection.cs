using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public class ExcelAttributeConnection : AttributeConnection
    {
        public ExcelAttributeConnection(Attribute attribute, BaseDataSourceDTO dataSource) : base(attribute, dataSource)
        {
        }

        public override IEnumerable<IAttributeDatum> GetData<T>() => throw new NotImplementedException();
    }
}
