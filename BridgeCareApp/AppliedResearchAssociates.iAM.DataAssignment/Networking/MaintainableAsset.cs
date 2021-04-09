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
        public MaintainableAsset(Guid id, Guid networkId, Location location, string spatialWeighting)
        {
            Id = id;
            NetworkId = networkId;
            Location = location;
            SpatialWeighting = spatialWeighting;
        }

        public AggregatedResult<T> GetAggregatedValuesByYear<T>(DataMinerAttribute attribute, AggregationRule<T> aggregationRule)
        {
            var specifiedData = AssignedData.Where(_ => _.Attribute.Id == attribute.Id);
            return new AggregatedResult<T>(Guid.NewGuid(), this, aggregationRule.Apply(specifiedData, attribute).ToList());
        }

        // TODO: side effect => mutate (get area equation; calculate spatial weighting)
        //public void AssignSpatialWeighting(string benefitQuantifierEquation)
        //{
        //    if (!AssignedData.Any() || !AssignedData.Any(_ => _ is AttributeDatum<double>))
        //    {
        //        return;
        //    }

        //    var numericAssignedData = AssignedData.Where(_ =>
        //        _ is AttributeDatum<double> && benefitQuantifierEquation.Contains(_.Attribute.Name)).ToList();

        //    var compiler = new CalculateEvaluateCompiler();
        //    foreach (var numericDatum in numericAssignedData.Cast<AttributeDatum<double>>())
        //    {
        //        compiler.ParameterTypes[numericDatum.Attribute.Name] = CalculateEvaluateParameterType.Number;
        //    }
        //    var calculator = compiler.GetCalculator(benefitQuantifierEquation);

        //    var scope = new CalculateEvaluateScope();
        //    foreach (var numericDatum in numericAssignedData.Cast<AttributeDatum<double>>())
        //    {
        //        scope.SetNumber(numericDatum.Attribute.Name, numericDatum.Value);
        //    }

        //    var result = calculator.Delegate(scope);
        //    //SpatialWeighting = new SpatialWeighting(result);
        //}

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

        public string SpatialWeighting { get; }

        public Location Location { get; }
    }
}
