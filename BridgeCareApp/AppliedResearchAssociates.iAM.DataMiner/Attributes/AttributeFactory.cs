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

                    attribute = new NumericAttribute(
                            Guid.NewGuid(),
                            attributeMetaDatum.Name,
                            defaultValue,
                            attributeMetaDatum.Maximum,
                            attributeMetaDatum.Minimum,
                            attributeMetaDatum.Command,
                            attributeMetaDatum.ConnectionType,
                            attributeMetaDatum.ConnectionString);

                    break;
                }
            case "TEXT":
                {
                    attribute = new TextAttribute(
                            Guid.NewGuid(),
                            attributeMetaDatum.Name,
                            attributeMetaDatum.DefaultValue,
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
