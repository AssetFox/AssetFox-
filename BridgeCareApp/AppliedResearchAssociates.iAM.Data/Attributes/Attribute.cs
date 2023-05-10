using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public abstract class Attribute : IEquatable<Attribute>
    {
        private static List<RuleDefinition> validAggregationRules = new List<RuleDefinition>() {
            new RuleDefinition { RuleName = "PREDOMINANT", IsText = true, IsNumeric = true },
            new RuleDefinition { RuleName = "AVERAGE", IsText = false, IsNumeric = true },
            new RuleDefinition { RuleName = "LAST", IsText = true, IsNumeric = true },
            new RuleDefinition { RuleName = "ADD", IsText = false, IsNumeric= true }
        };

        private static List<string> validDataTypes = new List<string>()
        {
            "NUMBER",
            "STRING"
        };

        public Attribute(Guid id,
            string name,
            string dataType,
            string ruleType,
            string command,
            ConnectionType connectionType,
            string connectionString,
            Guid? dataSourceId,
            bool isCalculated,
            bool isAscending)
        {
            Id = id;
            Name = name;
            if (!validDataTypes.Any(_ => _ == dataType.ToUpper()))
            {
                throw new InvalidOperationException($"Data type {dataType} is not valid for attribute {name}");
            }
            DataType = dataType;
            if (!validAggregationRules.Any(_ => _.RuleName == ruleType.ToUpper()))
            {
                throw new InvalidOperationException($"Attribute {name} cannot have a rule of {ruleType}");
            }
            AggregationRuleType = ruleType;
            Command = command;
            ConnectionType = connectionType;
            ConnectionString = connectionString;
            IsCalculated = isCalculated;
            IsAscending = isAscending;
            DataSourceId = dataSourceId;
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
        public Guid? DataSourceId { get; }

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

        public static List<RuleDefinition> AggregationRules
        {
            get { return validAggregationRules; }
        }

        public static List<string> DataTypes
        {
            get { return validDataTypes; }
        }
    }

    public class RuleDefinition
    {
        public string RuleName { get; set; }
        public bool IsText { get; set; }
        public bool IsNumeric { get; set; }
    }
}
