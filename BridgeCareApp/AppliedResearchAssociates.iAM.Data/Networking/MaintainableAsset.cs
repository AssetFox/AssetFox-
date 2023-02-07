﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.Data.Attributes;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Data.Networking
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

        public AggregatedResult<T> GetAggregatedValuesByYear<T>(Attribute attribute, AggregationRule<T> aggregationRule)
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

        public int AssignAttributeData(IEnumerable<IAttributeDatum> attributeData, Guid maintainableAssetId)
        {
            int unmatchedCount = 0;

            // Set up the log
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Maintainable asset unmatched datum - " + maintainableAssetId);
            StreamWriter streamWriter = new StreamWriter("D:\\Non-locationMatches_" + maintainableAssetId + ".txt");

            foreach (var datum in attributeData)
            {
                if (datum.Location.MatchOn(Location))
                {
                    AssignedData.Add(datum);
                }
                else
                {
                    // return the unmatched datum to be logged and reported
                    unmatchedCount++;
                    stringBuilder.AppendLine(datum.ToString());
                }
            }

            streamWriter.WriteLine(stringBuilder.ToString());
            streamWriter.Close();

            //string outputPath = /*Directory.GetCurrentDirectory()*/ "D:\\Non-locationMatches.txt";
            //File.CreateText(outputPath);
            //File.AppendAllText(outputPath, stringBuilder.ToString());
            //stringBuilder.Clear();

            return unmatchedCount;
        }

        public void AssignAttributeDataFromDataSource(IEnumerable<IAttributeDatum> attributeData) => AssignedData.AddRange(attributeData);

        public List<IAttributeDatum> AssignedData { get; } = new List<IAttributeDatum>();

        public Guid Id { get; }

        public Guid NetworkId { get; }

        public string SpatialWeighting { get; }

        public Location Location { get; }
    }
}
