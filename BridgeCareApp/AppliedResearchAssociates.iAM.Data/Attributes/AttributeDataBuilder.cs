using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public static class AttributeDataBuilder
    {
        public static IEnumerable<IAttributeDatum> GetData(AttributeConnection connection)
        {
            switch (connection.Attribute.DataType)
            {
            case "NUMBER":
                return connection.GetData<double>();

            case "STRING":
                return connection.GetData<string>();

            default:
                throw new InvalidOperationException();
            }
        }
    }
}
