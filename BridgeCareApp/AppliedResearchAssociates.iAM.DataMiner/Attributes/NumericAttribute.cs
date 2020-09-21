namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class NumericAttribute : Attribute
    {
        public NumericAttribute(string name,
                                double defaultValue,
                                double maximum,
                                double minimum,
                                string command,
                                ConnectionType connectionType,
                                string connectionString) : base(name, command, connectionType, connectionString)
        {
            DefaultValue = defaultValue;
            Maximum = maximum;
            Minimum = minimum;
        }

        public double DefaultValue { get; }

        public double Maximum { get; }

        public double Minimum { get; }
    }
}
