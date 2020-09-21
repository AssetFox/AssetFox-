using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Attribute
    {
        public Attribute(string name, string command, ConnectionType connectionType, string connectionString)
        {
            Guid = Guid.NewGuid();
            Name = name;
            Command = command;
            ConnectionType = connectionType;
            ConnectionString = connectionString;
        }

        public Guid Guid { get; }

        public string Name { get; }
        public string Command { get; }
        public ConnectionType ConnectionType { get; }
        public string ConnectionString { get; }
    }
}
