using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    // wjwjwj Write a test that invokes the GetData() method. Watch it in action.
    public class SqlAttributeConnection : AttributeConnection
    {
        public SqlAttributeConnection(Attribute attribute) : base(attribute)
        {
        }

        public override IEnumerable<IAttributeDatum> GetData<T>()
        {
            double? start = null;
            double? end = null;
            Direction? direction = null;
            string wellKnownText = null;

            using (var conn = new SqlConnection(Attribute.ConnectionString))
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
