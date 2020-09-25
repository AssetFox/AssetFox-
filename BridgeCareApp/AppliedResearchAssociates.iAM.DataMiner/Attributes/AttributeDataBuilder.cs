using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public static class AttributeDataBuilder
    {
        public static IEnumerable<IAttributeDatum> GetData(AttributeConnection connection)
        {
            switch(connection.Attribute.DataType)
            {
            case "NUMERIC":
                return connection.GetData<double>();
            case "TEXT":
                return connection.GetData<string>();
            default:
                throw new InvalidOperationException();
            }
        }
    }
}
