using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class TextAttribute : Attribute
    {
        public TextAttribute(Guid guid, string name, string defaultValue, string command, ConnectionType connectionType, string connectionString) : base(guid, name, command, "TEXT", connectionType, connectionString)
        {
            DefaultValue = defaultValue;
        }

        public string DefaultValue { get; }
    }
}
