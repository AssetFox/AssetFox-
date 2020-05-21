﻿using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class SqlAttributeConnection : AttributeConnection
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        public string DataSource { get; set; }

        public SqlAttributeConnection(string userName, string password, string server, string dataSource)
        {
            UserName = userName;
            Password = password;
            Server = server;
            DataSource = dataSource;
        }

        public override void Connect()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Maps data from a data source to routeName, start, end,
        ///     direction, and/or wellKnownText. The only required value from
        ///     the data set is a uniqueIdentifier
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override IEnumerable<(Location location, T value)> GetData<T>()
        {
            string routeName = null;
            double? start = null;
            double? end = null;
            Direction? direction = null;
            string wellKnownText = null;

            foreach (var datum in data)
            {
                string uniqueIdentifier = data.UniqueIdentifier;
                yield return (LocationBuilder.CreateLocation
                    (
                        uniqueIdentifier,
                        routeName,
                        start,
                        end,
                        direction,
                        wellKnownText
                    ),
                    datum.Value);
            }
        }
    }
}
