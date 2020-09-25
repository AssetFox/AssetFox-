using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Aggregation;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataAccess
{
    public sealed class DataAccessorModified
    {
        public Segmentation.Network CreateNewSegmentation()
        {
            var segmentationRulesJsonText = File.ReadAllText("segmentationMetaData.json");
            var segmentationRulesMetaData = JsonConvert.DeserializeAnonymousType(segmentationRulesJsonText, new { SegmentationRules = default(SegmentationMetaDatum) }).SegmentationRules;

            switch (segmentationRulesMetaData.DataType)
            {
                case "NUMERIC":
                {
                    if (!double.TryParse(segmentationRulesMetaData.DefaultValue, out double defaultValue))
                    {
                        throw new InvalidCastException($"Numeric attribute {segmentationRulesMetaData.AttributeName} does not have a valid numeric default value. Please check the value in the configuration file and try again.");
                    }

                    var attribute = new NumericAttribute(
                            Guid.NewGuid(),
                            segmentationRulesMetaData.AttributeName,
                            defaultValue,
                            segmentationRulesMetaData.Maximum,
                            segmentationRulesMetaData.Minimum,
                            segmentationRulesMetaData.DataRetrievalCommand,
                            segmentationRulesMetaData.ConnectionType,
                            segmentationRulesMetaData.ConnectionString);

                    var attributeData = AttributeConnectionBuilder.Create(attribute).GetData<double>();
                    return Segmenter.CreateNetworkFromAttributeDataRecords<double>(attributeData);
                }
                case "TEXT":
                {
                    var attribute = new DataMiner.Attributes.TextAttribute(
                            Guid.NewGuid(),
                            segmentationRulesMetaData.AttributeName,
                            segmentationRulesMetaData.DefaultValue,
                            segmentationRulesMetaData.DataRetrievalCommand,
                            segmentationRulesMetaData.ConnectionType,
                            segmentationRulesMetaData.ConnectionString);

                    var attributeData = AttributeConnectionBuilder.Create(attribute).GetData<string>();
                    return Segmenter.CreateNetworkFromAttributeDataRecords<string>(attributeData);
                }
                default:
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void AggregateExistingNetwork(Guid networkGuid)
        {
            var network = GetNetwork(networkGuid);

            var attributeJsonText = File.ReadAllText("attributeMetaData.json");
            var attributeMetaData = JsonConvert.DeserializeAnonymousType(attributeJsonText, new { AttributeMetaData = default(List<AttributeMetaDatum>) }).AttributeMetaData;

            var attributeData = new List<IAttributeDatum>();
            var attributes = new List<DataMiner.Attributes.Attribute>();

            // Create the list of attributes
            foreach(var attributeMetaDatum in attributeMetaData)
            {
                var attribute = AttributeFactory.Create(attributeMetaDatum);
                attributes.Add(attribute);
            }

            // Create the attribute data for each attribute
            foreach(var attribute in attributes)
            {
                attributeData.AddRange(AttributeDataFactory.GetData(AttributeConnectionBuilder.Create(attribute)));
            }

            // Assign the attribute data to segments
            var segments = Aggregator.AssignAttributeDataToSegments(attributeData, network.Segments);

            foreach(var attribute in attributes)
            {
                foreach(var segment in segments)
                {
                    segment.GetAggregatedValuesByYear
                }
            }
            // Save to database; Use in analysis.
        }

        private Segmentation.Network GetNetwork(Guid networkGuid)
        {
            throw new NotImplementedException();
        }
    }
}
