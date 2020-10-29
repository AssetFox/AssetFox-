using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
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
                    if (!(dataReader["DATA_"] is DBNull || dataReader["DATE_"] is DBNull || dataReader["FACILITY"] is DBNull ||
                        dataReader["SECTION"] is DBNull))
                    {
                        var value = (T)Convert.ChangeType(dataReader["DATA_"], typeof(T));
                        var dateTime = Convert.ToDateTime(dataReader["DATE_"]);
                        var uniqueIdentifier = $"{dataReader["FACILITY"]}{dataReader["SECTION"]}";

                        yield return new AttributeDatum<T>(Guid.NewGuid(), Attribute, value,
                            LocationBuilder.CreateLocation(uniqueIdentifier, start, end, direction, wellKnownText),
                            dateTime);
                    }
                }
            }
        }
    }
}
