using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public static class AttributeConnectionBuilder
    {
        public static AttributeConnection Create(ConnectionType connectionType, string connectionString, string command)
        {
            switch (connectionType)
            {
            case ConnectionType.MSSQL:
                return new SqlAttributeConnection(connectionString, command);
            case ConnectionType.MONGO_DB:
                throw new NotImplementedException("Mongo Db data retrival has not been implemented");
            default:
                throw new InvalidOperationException($"Invalid Connection type \"{connectionType}\".");
            }
        }
    }
}
