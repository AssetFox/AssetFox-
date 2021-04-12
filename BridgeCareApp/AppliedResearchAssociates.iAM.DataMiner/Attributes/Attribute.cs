using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Attribute : IEquatable<Attribute>
    {
        public Attribute(Guid id,
            string name,
            string dataType,
            string ruleType,
            string command,
            ConnectionType connectionType,
            string connectionString,
            bool isCalculated,
            bool isAscending)
        {
            Id = id;
            Name = name;
            DataType = dataType;
            AggregationRuleType = ruleType;
            Command = command;
            ConnectionType = connectionType;
            ConnectionString = connectionString;
            IsCalculated = isCalculated;
            IsAscending = isAscending;
        }

        public Guid Id { get; }
        public string DataType { get; }
        public string Name { get; }
        public string AggregationRuleType { get; }
        public string Command { get; }
        public ConnectionType ConnectionType { get; }
        public string ConnectionString { get; }
        public bool IsCalculated { get; set; }
        public bool IsAscending { get; set; }

        public bool Equals(Attribute other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is Attribute a && Equals(a);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
