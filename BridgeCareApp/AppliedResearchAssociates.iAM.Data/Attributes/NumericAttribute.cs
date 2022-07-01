using System;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public class NumericAttribute : Attribute
    {
        public NumericAttribute(double defaultValue,
            double? maximum,
            double? minimum,
            Guid id,
            string name,
            string ruleType,
            string command,
            ConnectionType connectionType,
            string connectionString,
            bool isCalculated,
            bool isAscending,
            Guid? dataSourceId)
            : base(id, name, "NUMBER", ruleType, command, connectionType, connectionString, dataSourceId, isCalculated, isAscending)
        {
            DefaultValue = defaultValue;
            Maximum = maximum;
            Minimum = minimum;
        }

        public double DefaultValue { get; }

        public double? Maximum { get; }

        public double? Minimum { get; }
    }
}
