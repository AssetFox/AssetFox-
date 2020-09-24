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
        public void CreateNewSegmentation()
        {
            var segmentationRulesJsonText = File.ReadAllText("segmentationMetaData.json");
            var segmentationRulesMetaData = JsonConvert.DeserializeAnonymousType(segmentationRulesJsonText, new { SegmentationRules = default(SegmentationMetaDatum) }).SegmentationRules;

            switch (segmentationRulesMetaData.DataType)
            {
                case "number":
                {
                    if (!double.TryParse(segmentationRulesMetaData.DefaultValue, out double defaultValue))
                    {
                        throw new InvalidCastException($"Numeric attribute {segmentationRulesMetaData.AttributeName} does not have a valid numeric default value. Please check the value in the configuration file and try again.");
                    }

                    var attribute = new NumericAttribute(
                            segmentationRulesMetaData.AttributeName,
                            defaultValue,
                            segmentationRulesMetaData.Maximum,
                            segmentationRulesMetaData.Minimum,
                            segmentationRulesMetaData.DataRetrievalCommand,
                            segmentationRulesMetaData.ConnectionType,
                            segmentationRulesMetaData.ConnectionString);

                    var attributeData = AttributeConnectionBuilder.Create(attribute).GetData<double>();
                    CreateNetworkFromSingleAttribute<double>(attributeData);

                    // Save to the database

                    break;
                }
                case "TEXT":
                {
                    var attribute = new DataMiner.Attributes.TextAttribute(
                            segmentationRulesMetaData.AttributeName,
                            segmentationRulesMetaData.DefaultValue,
                            segmentationRulesMetaData.DataRetrievalCommand,
                            segmentationRulesMetaData.ConnectionType,
                            segmentationRulesMetaData.ConnectionString);

                    var attributeData = AttributeConnectionBuilder.Create(attribute).GetData<string>();
                    CreateNetworkFromSingleAttribute<double>(attributeData);

                    // Save to the database

                    break;
                }
            }
        }

        public void AggregateExistingNetwork(Guid networkGuid)
        {
            var network = GetNetwork(networkGuid);

            var attributeJsonText = File.ReadAllText("attributeMetaData.json");
            var attributeMetaData = JsonConvert.DeserializeAnonymousType(attributeJsonText, new { AttributeMetaData = default(List<AttributeMetaDatum>) }).AttributeMetaData;

            var attributeData = new List<IAttributeDatum>();

            foreach(var attributeMetaDatum in attributeMetaData)
            {
                switch (attributeMetaDatum.Type)
                {
                    case "NUMERIC":
                    {
                        if (!double.TryParse(attributeMetaDatum.DefaultValue, out double defaultValue))
                        {
                            throw new InvalidCastException($"Numeric attribute {attributeMetaDatum.Name} does not have a valid numeric default value. Please check the value in the configuration file and try again.");
                        }

                        var attribute = new NumericAttribute(
                                attributeMetaDatum.Name,
                                defaultValue,
                                attributeMetaDatum.Maximum,
                                attributeMetaDatum.Minimum,
                                attributeMetaDatum.Command,
                                attributeMetaDatum.ConnectionType,
                                attributeMetaDatum.ConnectionString);

                        attributeData.AddRange(AttributeConnectionBuilder.Create(attribute).GetData<double>());
                        break;
                    }
                    case "TEXT":
                    {
                        var attribute = new DataMiner.Attributes.TextAttribute(
                                attributeMetaDatum.Name,
                                attributeMetaDatum.DefaultValue,
                                attributeMetaDatum.Command,
                                attributeMetaDatum.ConnectionType,
                                attributeMetaDatum.ConnectionString);

                        attributeData.AddRange(AttributeConnectionBuilder.Create(attribute).GetData<string>());
                        break;
                    }
                }
            }
            var aggregatedDataSegments = Aggregator.Aggregate(attributeData, network.Segments);

            // Save to database; Use in analysis.
        }

        public void RunAnalysis(Guid scenarioGuid)
        {

        }

        private Segmentation.Network CreateNetworkFromSingleAttribute<T>(IEnumerable<IAttributeDatum> attributeData)
        {
            return Segmenter.CreateNetworkFromAttributeDataRecords<T>(attributeData);
        }

        private Segmentation.Network GetNetwork(Guid networkGuid)
        {
            throw new NotImplementedException();
        }
    }
}
