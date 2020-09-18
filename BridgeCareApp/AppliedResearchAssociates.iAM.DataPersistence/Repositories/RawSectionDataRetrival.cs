using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL
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

        public IEnumerable<(Location location, T value)> GetRawData<T>()
        {
            switch (ConnectionType)
            {
            case "MSSQL":
                var sqlConnection = new SqlAttributeConnection(ConnectionString, DataRetrivalCommand);
                AttributeConnection = sqlConnection;
                var rawAttributeData = sqlConnection.GetData<T>();
                return rawAttributeData;
            case "MongoDB":
                throw new NotImplementedException("Mongo Db data retrival has not been implemented");
                break;
            default:
                throw new InvalidOperationException($"Invalid Connection type \"{ConnectionType}\".");
            }
        }

        public NumericAttribute GetNumericAttribute(string attributeName, double defaultValue, double maximum, double minimum)
        {
            return new NumericAttribute(attributeName, AttributeConnection, defaultValue, maximum, minimum); 
        }

        public TextAttribute GetTextAttribute(string name, string defaultValue)
        {
            return new TextAttribute(name, AttributeConnection, defaultValue);
        }
    }
}
