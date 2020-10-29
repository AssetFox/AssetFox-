using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Attribute
    {
        public Attribute(Guid id,
            string name,
            string dataType,
            string ruleType,
            string command,
            ConnectionType connectionType,
            string connectionString)
        {
            Id = id;
            Name = name;
            DataType = dataType;
            AggregationRuleType = ruleType;
            Command = command;
            ConnectionType = connectionType;
            ConnectionString = connectionString;
        }

        public Guid Id { get; }
        public string DataType { get; }
        public string Name { get; }
        public string AggregationRuleType { get; }
        public string Command { get; }
        public ConnectionType ConnectionType { get; }
        public string ConnectionString { get; }
    }
}
