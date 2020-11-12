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
            string connectionString,
            bool isCalculated,
            bool isAscending)
            : base(id, name, "NUMERIC", ruleType, command, connectionType, connectionString, isCalculated, isAscending)
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
