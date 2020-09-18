using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public class RawSectionDataRetrival
    {
        public string ConnectionType { get; }
        public string ConnectionString { get; }
        public string DataRetrivalCommand { get; }
        public AttributeConnection AttributeConnection { get; private set; }
        public RawSectionDataRetrival(string connectionType, string connectionString, string dataRetrivalCommand)
        {
            ConnectionType = connectionType;
            ConnectionString = connectionString;
            DataRetrivalCommand = dataRetrivalCommand;
        }

        public AttributeConnection GetConnection()
        {
            switch (ConnectionType)
            {
            case "MSSQL":
                var sqlConnection = new SqlAttributeConnection(ConnectionString, DataRetrivalCommand);
                AttributeConnection = sqlConnection;
                return AttributeConnection;
            case "MongoDB":
                throw new NotImplementedException("Mongo Db data retrival has not been implemented");
                break;
            default:
                throw new InvalidOperationException($"Invalid Connection type \"{ConnectionType}\".");
            }
        }
    }
}
