using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class NumericAttribute : Attribute
    {
        public NumericAttribute(Guid guid,
                                string name,
                                double defaultValue,
                                double maximum,
                                double minimum,
                                string command,
                                ConnectionType connectionType,
                                string connectionString) : base(guid, name, command, "NUMERIC", connectionType, connectionString)
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
