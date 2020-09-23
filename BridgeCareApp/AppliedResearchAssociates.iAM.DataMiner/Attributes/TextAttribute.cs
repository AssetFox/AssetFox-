namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class TextAttribute : Attribute
    {
        public TextAttribute(string name, string defaultValue, string command, ConnectionType connectionType, string connectionString) : base(name, command, connectionType, connectionString)
        {
            DefaultValue = defaultValue;
        }

        public string DefaultValue { get; }
    }
}
