using System;
using AppliedResearchAssociates.iAM.Data.Attributes;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Data
{
    public static class AttributeConnectionBuilder
    {
        public static AttributeConnection Build(Attribute attribute)
        {
            switch (attribute.ConnectionType)
            {
            case ConnectionType.MSSQL:
                return new SqlAttributeConnection(attribute);

            case ConnectionType.EXCEL:
                throw new NotImplementedException("Mongo Db data retrieval has not been implemented");
            default:
                throw new InvalidOperationException($"Invalid Connection type \"{attribute.ConnectionType}\".");
            }
        }
    }
}
