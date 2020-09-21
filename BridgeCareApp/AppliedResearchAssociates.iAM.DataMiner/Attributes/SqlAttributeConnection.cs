using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class SqlAttributeConnection : AttributeConnection
    {
        public SqlAttributeConnection(string connectionString, string command) : base(connectionString, command)
        {
        }

        public override IEnumerable<IAttributeDatum> GetData<T>()
        {
            string routeName = null;
            double? start = null;
            double? end = null;
            Direction? direction = null;
            string wellKnownText = null;
            string uniqueIdentifeir = "";

            using (var conn = new SqlConnection())
            {
                var sqlCommand = new SqlCommand(Command, conn);
                sqlCommand.Connection.Open();
                var dataReader = sqlCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    var value = dataReader.GetFieldValue<T>(5);
                    var dateTime = dataReader.GetFieldValue<DateTime>(6);

                    yield return
                        new AttributeDatum<T>(Attribute,
                                              value,
                                              LocationBuilder.CreateLocation(uniqueIdentifeir, routeName, start, end, direction, wellKnownText),
                                              dateTime);    
                }
            }
        }
    }
}
