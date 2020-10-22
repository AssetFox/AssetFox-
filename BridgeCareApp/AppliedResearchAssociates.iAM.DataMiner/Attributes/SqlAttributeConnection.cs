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

        public override event EventHandler<InformationEventArgs> Information;

        public override IEnumerable<IAttributeDatum> GetData<T>()
        {
            double? start = null;
            double? end = null;
            Direction? direction = null;
            string wellKnownText = null;
            using (var conn = new SqlConnection(Attribute.ConnectionString))
            {
                Inform("Fetching arrtibute data");
                var sqlCommand = new SqlCommand(Attribute.Command, conn);
                sqlCommand.Connection.Open();
                var dataReader = sqlCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    var value = dataReader.GetFieldValue<T>(2);
                    var dateTime = dataReader.GetFieldValue<DateTime>(1);
                    var uniqueIdentifier = dataReader.GetFieldValue<string>(0);
                    yield return
                        new AttributeDatum<T>(Attribute,
                                              value,
                                              LocationBuilder.CreateLocation(uniqueIdentifier, start, end, direction, wellKnownText),
                                              dateTime);
                }
            }
        }

        internal void Inform(string message) => OnInformation(new InformationEventArgs(message));

        private void OnInformation(InformationEventArgs e) => Information?.Invoke(this, e);
    }
}
