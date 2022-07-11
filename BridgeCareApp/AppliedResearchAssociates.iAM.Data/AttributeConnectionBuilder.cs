using System;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Data
{
    public static class AttributeConnectionBuilder
    {
        public static AttributeConnection Build(Attribute attribute, BaseDataSourceDTO dataSource)
        {
            // wjwjwj run this thing.
            switch (attribute.ConnectionType)
            {
            case ConnectionType.MSSQL:
                return new SqlAttributeConnection(attribute, dataSource);

            case ConnectionType.EXCEL:
                throw new NotImplementedException("Excel data retrieval has not been implemented");
            default:
                throw new InvalidOperationException($"Invalid Connection type \"{attribute.ConnectionType}\".");
            }
        }
    }
}
