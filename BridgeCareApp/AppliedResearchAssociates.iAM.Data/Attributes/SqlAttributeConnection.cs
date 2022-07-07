using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public class SqlAttributeConnection : AttributeConnection
    {
        // WjTodo this is the class we need to test in item 19085.
        // First, test if the connection works.
        // Then, test if the command works.

        // Queries for an attribute should like this:
        // "SELECT CAST(BRKEY AS VARCHAR(MAX)) & '-' & BRIDGE_ID AS LOCATION_IDENTIFIER, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(REPLACE(ADTTOTAL, ',', '') AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (ISNUMERIC(ADTTOTAL) = 1)",

        public const string DateColumnName = "DATE_";
        public const string DataColumnName = "DATA_";
        public const string LocationIdentifierString = "LOCATION_IDENTIFIER";

        public SqlAttributeConnection(Attribute attribute) : base(attribute)
        {
        }

        public override IEnumerable<IAttributeDatum> GetData<T>()
        {
            double? start = null;
            double? end = null;
            Direction? direction = null;
            string wellKnownText = null;
            var connectionString = Attribute.ConnectionString;


            using (var conn = new SqlConnection(connectionString))
            {
                var sqlCommand = new SqlCommand(Attribute.Command, conn);
                sqlCommand.Connection.Open();
                var dataReader = sqlCommand.ExecuteReader();

                while (dataReader.Read())
                {
                    if (!(dataReader[DataColumnName] is DBNull || dataReader[DateColumnName] is DBNull || dataReader[LocationIdentifierString] is DBNull))
                    {
                        var value = (T)Convert.ChangeType(dataReader[DataColumnName], typeof(T));
                        var dateTime = Convert.ToDateTime(dataReader[DateColumnName]);
                        var locationIdentifier = dataReader[LocationIdentifierString].ToString();

                        yield return new AttributeDatum<T>(Guid.NewGuid(), Attribute, value,
                            LocationBuilder.CreateLocation(locationIdentifier, start, end, direction, wellKnownText),
                            dateTime);
                    }
                }
            }
        }
    }
}
