using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public class Attribute
    {
        public string ID { get; set; }
    }
    public class Attribute<T> : Attribute
    {
        public double DefaultValue { get; set; }
    }
    public sealed class NumberAttribute : Attribute<double>
    {
        public bool IsDecreasingWithDeterioration { get; set; }
        public double? MinimumValue { get; set; }
        public double? MaximumValue { get; set; }
    }

    public sealed class TextAttribute : Attribute<string>
    {
    }

    public sealed class CalculatedField : Attribute
    {
        public bool IsDecreasingWithDeterioration { get; set; }
        public CalculatedFieldTiming Timing { get; set; }
        public List<ConditionalValueDefinition> ValueDefinitions { get; set; }
    }
    public sealed class ConditionalValueDefinition
    {
        public string ConditionExpression { get; set; }
        public string ValueExpression { get; set; }
    }

    public sealed class SystemDefinition
    {
        public List<NumberAttribute> NumberAttributes { get; set; }
        public List<TextAttribute> TextAttributes { get; set; }
        public List<CalculatedField> CalculatedFields { get; set; }
        public List<Network> Networks { get; set; }
    }

    public sealed class Network
    {
        public string ID { get; set; }
        public List<Asset> Assets { get; set; }
        public List<Scenario> Scenarios { get; set; }
    }

    public sealed class Asset
    {
        public string ID { get; set; }
        public string SpatialWeightExpression { get; set; }
        public List<AttributeValueHistory<double>> NumberAttributeHistories { get; set; }
        public List<AttributeValueHistory<string>> TextAttributeHistories { get; set; }
    }

    public sealed class AttributeValueHistory<T>
    {
        public string AttributeID { get; set; }
        public List<HistoricalValue<T>> History { get; set; }
    }

    public sealed class HistoricalValue<T>
    {
        public int Year { get; set; }
        public T Value { get; set; }
    }

    public sealed class Scenario
    {
        public AnalysisMethod AnalysisMethod { get; set; }
    }

    public sealed class AnalysisMethod
    {
        public string BenefitAttributeID { get; set; }
        public double BenefitLimit { get; set; }
        public string BenefitWeightAttributeID { get; set; }
        public string FilterExpression { get; set; }
    }
}
