using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public static class AttributeFactory
    {
        public static Attribute Create(AttributeMetaDatum attributeMetaDatum)
        {
            Attribute attribute;

            switch (attributeMetaDatum.Type)
            {
                case "NUMERIC":
                    {
                        if (!double.TryParse(attributeMetaDatum.DefaultValue, out var defaultValue))
                        {
                            throw new InvalidCastException($"Numeric attribute {attributeMetaDatum.Name} does not have a valid numeric default value. Please check the value in the configuration file and try again.");
                        }

                        attribute = new NumericAttribute(defaultValue,
                            attributeMetaDatum.Maximum,
                            attributeMetaDatum.Minimum,
                            Guid.NewGuid(),
                            attributeMetaDatum.Name,
                            attributeMetaDatum.AggregationRule,
                            attributeMetaDatum.Command,
                            attributeMetaDatum.ConnectionType,
                            attributeMetaDatum.ConnectionString);

                        break;
                    }
                case "TEXT":
                    {
                        attribute = new TextAttribute(attributeMetaDatum.DefaultValue,
                            Guid.NewGuid(),
                            attributeMetaDatum.Name,
                            attributeMetaDatum.AggregationRule,
                            attributeMetaDatum.Command,
                            attributeMetaDatum.ConnectionType,
                            attributeMetaDatum.ConnectionString);

                        break;
                    }
                default:
                    throw new InvalidOperationException();
            }

            return attribute;
        }
    }
}
