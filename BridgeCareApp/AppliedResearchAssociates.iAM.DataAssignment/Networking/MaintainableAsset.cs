using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = System.Attribute;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataAssignment.Networking
{
    public class MaintainableAsset
    {
        public MaintainableAsset(Guid id, Guid networkId, Location location)
        {
            Id = id;
            NetworkId = networkId;
            Location = location;
        }

        public AggregatedResult<T> GetAggregatedValuesByYear<T>(DataMinerAttribute attribute, AggregationRule<T> aggregationRule)
        {
            var specifiedData = AssignedData.Where(_ => _.Attribute.Id == attribute.Id);
            return new AggregatedResult<T>(Guid.NewGuid(), this, aggregationRule.Apply(specifiedData, attribute));
        }

        // TODO: side effect => mutate (get area equation; calculate spatial weighting)
        public void AssignSpatialWeighting(List<DataMinerAttribute> attributes, string benefitQuantifierEquation)
        {
            if (!AssignedData.Any() || !AssignedData.Any(_ => _ is AttributeDatum<double>))
            {
                return;
            }
            // run spatial weighting equation to assign SpatialWeighting value here
            var stringAttributes = attributes.Where(_ => _.DataType == "STRING").ToList();
            var numberAttributes = attributes.Where(_ => _.DataType == "NUMBER").ToList();
            CheckEquationAttributes(stringAttributes, numberAttributes, benefitQuantifierEquation);

            var numericAssignedData = AssignedData.Where(_ =>
                _ is AttributeDatum<double> && benefitQuantifierEquation.Contains(_.Attribute.Name)).ToList();

            var compiler = new CalculateEvaluateCompiler();
            foreach (var numericDatum in numericAssignedData.Cast<AttributeDatum<double>>())
            {
                compiler.ParameterTypes[numericDatum.Attribute.Name] = CalculateEvaluateParameterType.Number;
            }
            var calculator = compiler.GetCalculator(benefitQuantifierEquation);

            var scope = new CalculateEvaluateScope();
            foreach (var numericDatum in numericAssignedData.Cast<AttributeDatum<double>>())
            {
                scope.SetNumber(numericDatum.Attribute.Name, numericDatum.Value);
            }

            var result = calculator.Delegate(scope);
            SpatialWeighting = new SpatialWeighting(result);
        }

        private void CheckEquationAttributes(List<DataMinerAttribute> stringAttributes,
            List<DataMinerAttribute> numberAttributes, string target)
        {
            if (stringAttributes.Any(_ => target.Contains(_.Name)))
            {
                var stringAttributesInEquation = stringAttributes.Where(_ => target.Contains(_.Name)).ToList();
                throw new InvalidOperationException(
                    $"Unsupported string attributes found in benefit quantifier equation expression: {string.Join(", ", stringAttributesInEquation)}.");
            }

            target = target.Replace('[', '?');
            foreach (var allowedAttribute in numberAttributes.Where(allowedAttribute => target.IndexOf("?" + allowedAttribute.Name + "]", StringComparison.Ordinal) >= 0))
            {
                target = target.Replace("?" + allowedAttribute.Name + "]", "[" + allowedAttribute.Name + "]");
            }

            if (target.Count(f => f == '?') <= 0)
            {
                return;
            }

            var invalidAttributes = new List<string>();

            do
            {
                var start = target.IndexOf('?');
                var end = target.IndexOf(']');
                var invalidAttribute = target.Substring(start + 1, end - 1);
                invalidAttributes.Add(invalidAttribute);
                var invalidAttributePosition = target.IndexOf($"?{invalidAttribute}]", StringComparison.Ordinal);
                target = invalidAttributePosition + 1 <= target.Length
                    ? target.Substring(invalidAttributePosition + 1)
                    : "";
            } while (target.Contains("?"));

            throw new InvalidOperationException(
                $"Unsupported attribute(s) found in benefit quantifier equation expression: {string.Join(", ", invalidAttributes)}");
        }

        public void AssignAttributeData(IEnumerable<IAttributeDatum> attributeData)
        {
            foreach (var datum in attributeData)
            {
                if (datum.Location.MatchOn(Location))
                {
                    AssignedData.Add(datum);
                }
                else
                {
                    // TODO: No matching maintainable asset for the current data. What do we do?
                }
            }
        }

        public void AssignAttributeDataFromDataSource(IEnumerable<IAttributeDatum> attributeData) => AssignedData.AddRange(attributeData);

        public List<IAttributeDatum> AssignedData { get; } = new List<IAttributeDatum>();

        public Guid Id { get; }

        public Guid NetworkId { get; }

        public SpatialWeighting SpatialWeighting { get; set; }

        public Location Location { get; }
    }
}
