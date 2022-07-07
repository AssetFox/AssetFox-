using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public class ExcelAttributeConnection : AttributeConnection
    {
        public ExcelAttributeConnection(Attribute attribute) : base(attribute)
        {
        }

        public override IEnumerable<IAttributeDatum> GetData<T>() => throw new NotImplementedException();
    }
}
