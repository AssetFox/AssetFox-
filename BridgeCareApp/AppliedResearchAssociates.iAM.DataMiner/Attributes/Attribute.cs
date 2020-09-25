using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Attribute
    {
        public Attribute(Guid guid, string name, string command, string dataType, ConnectionType connectionType, string connectionString)
        {
            Guid = guid;
            Name = name;
            DataType = dataType;
            Command = command;
            ConnectionType = connectionType;
            ConnectionString = connectionString;
        }

        public Guid Guid { get; }

        public string DataType { get; }
        public string Name { get; }
        public string Command { get; }
        public ConnectionType ConnectionType { get; }
        public string ConnectionString { get; }
    }
}
