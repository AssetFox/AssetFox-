using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class NumericAttribute : Attribute
    {
        public NumericAttribute(double defaultValue,
            double maximum,
            double minimum,
            Guid id,
            string name,
            string ruleType,
            string command,
            ConnectionType connectionType,
            string connectionString)
            : base(id, name, "NUMERIC", ruleType, command, connectionType, connectionString)
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
