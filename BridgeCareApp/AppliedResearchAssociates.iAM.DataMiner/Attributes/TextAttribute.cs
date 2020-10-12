using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class TextAttribute : Attribute
    {
        public TextAttribute(string defaultValue,
            Guid id,
            string name,
            string ruleType,
            string command,
            ConnectionType connectionType,
            string connectionString)
            : base(id, name, "TEXT", ruleType, command, connectionType, connectionString) =>
            DefaultValue = defaultValue;

        public string DefaultValue { get; }
    }
}
