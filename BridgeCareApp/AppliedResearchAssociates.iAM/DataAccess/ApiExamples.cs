﻿using System;
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
    public sealed class ApiExamples
    {
        public Segmentation.Network CreateNewSegmentation()
        {
            var segmentationRulesJsonText = File.ReadAllText("segmentationMetaData.json");
            var attributeMetaDatum = JsonConvert.DeserializeAnonymousType(segmentationRulesJsonText, new { AttributeMetaDatum = default(AttributeMetaDatum) }).AttributeMetaDatum;

            var attribute = AttributeFactory.Create(attributeMetaDatum);
            var attributeData = AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute));

            return Segmenter.CreateNetworkFromAttributeDataRecords(attributeData);
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
                attributeData.AddRange(AttributeDataBuilder.GetData(AttributeConnectionBuilder.Build(attribute)));
            }

            // Assign the attribute data to segments
            var segments = Aggregator.AssignAttributeDataToSegments(attributeData, network.Segments);

            var aggregatedNumericResults = new List<(DataMiner.Attributes.Attribute attribute, (int year, double value))>();
            var aggregatedTextResults = new List<(DataMiner.Attributes.Attribute attribute, (int year, string value))>();
            foreach (var attribute in attributes)
            {
                foreach (var segment in segments)
                {
                    switch (attribute.DataType)
                    {
                    case "NUMERIC":
                        aggregatedNumericResults.AddRange(segment.GetAggregatedValuesByYear(attribute, AggregationRuleFactory.CreateNumericRule(attribute)));
                        break;
                    case "TEXT":
                        aggregatedTextResults.AddRange(segment.GetAggregatedValuesByYear(attribute, AggregationRuleFactory.CreateTextRule(attribute)));
                        break;
                    }
                   
                
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
