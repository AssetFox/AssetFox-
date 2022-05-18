using System;

namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public static class AttributeFactory
    {
        public static Attribute Create(AttributeMetaDatum available)
        {
            Attribute attribute;
            switch (available.Type)
            {
            case "NUMBER":
                {
                    if (!double.TryParse(available.DefaultValue, out var defaultValue))
                    {
                        throw new InvalidCastException($"Numeric attribute {available.Name} does not have a valid numeric default value. Please check the value in the configuration file and try again.");
                    }

                    attribute = new NumericAttribute(defaultValue,
                        available.Maximum,
                        available.Minimum,
                        available.Id,
                        available.Name,
                        available.AggregationRule,
                        available.Command,
                        available.ConnectionType,
                        available.ConnectionString,
                        available.IsCalculated,
                        available.IsAscending);

                    break;
                }
            case "STRING":
                {
                    attribute = new TextAttribute(available.DefaultValue,
                        available.Id,
                        available.Name,
                        available.AggregationRule,
                        available.Command,
                        available.ConnectionType,
                        available.ConnectionString,
                        available.IsCalculated,
                        available.IsAscending);

                    break;
                }
            default:
                throw new InvalidOperationException();
            }

            return attribute;
        }
    }
}
