using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AppliedResearchAssociates.iAM.Aggregation;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistence.Models;
using AppliedResearchAssociates.iAM.DataPersistence.Repositories;
using AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.Segmentation;
using BridgeCare.Models.AggregationObjects;
using Network = AppliedResearchAssociates.iAM.Segmentation.Network;

namespace BridgeCare.Domain
{
    public class Aggregation
    {
        private readonly IRepository<AttributeMetaDatum> attributeRepository;

        private const string NUMBER_ATTRIBUTE_TYPE_NAME = "NUMBER";

        private const string STRING_ATTRIBUTE_TYPE_NAME = "STRING";

        public Aggregation(IRepository<AttributeMetaDatum> attributeRepository)
        {
            this.attributeRepository = attributeRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "<Pending>")]
        internal Task<string> Run(Network network)
        {
            var attributeMetaData = attributeRepository.All();

            var aggregatedSegments = new List<AssignedDataSegment>();
            
            // Get the attribute data and start aggregating
            //foreach (var item in attributeMetaData)
            //{
            //    var type = item.DataType;
            //    var name = item.AttributeName;
            //    var dataRetrival = new AttributeDataProvider(item.DataSource, item.ConnectionString, item.DataRetrievalCommand);
            //    switch (type.ToUpper())
            //    {
            //    case NUMBER_ATTRIBUTE_TYPE_NAME:

            //        //Data miner
            //        var rawAttributeData = dataRetrival.GetRawData<double>();

            //        var numericAttribute = dataRetrival.GetNumericAttribute(name, Convert.ToDouble(item.DefaultValue),
            //                                                     item.Maximum, item.Minimum);

            //        var attributeData = AttributeDatumBuilder<double>.CreateAttributeData(numericAttribute, rawAttributeData);

            //        // Aggregation
            //        var aggregateDataSegments = Aggregator.Aggregate(attributeData, network.Segments);
            //        foreach(var aggregateDataSegment in aggregateDataSegments)
            //        {
            //            var attributeYearValue = aggregateDataSegment.GetAggregatedValuesByYear(numericAttribute, new AverageAggregationRule()); // it should return the whole object
            //        }
            //        aggregatedSegments.AddRange(aggregateDataSegments); // we need to save the network also with EF core
                    
            //        break;

            //    case STRING_ATTRIBUTE_TYPE_NAME:

            //        // Data miner
            //        var rawData = dataRetrival.GetRawData<string>();
            //        var textAttribute = dataRetrival.GetTextAttribute(name, item.DefaultValue);
            //        var textAttributeDatum = AttributeDatumBuilder<string>.CreateAttributeData(textAttribute, rawData);

            //        // Aggregation
            //        var textData = new List<IAttributeDatum>();
            //        textData.AddRange(textAttributeDatum);
            //        var textAggregation = Aggregator.Aggregate(textData, network.Segments);
            //        aggregatedSegments.AddRange(textAggregation);
            //        break;

            //    default:
            //        throw new InvalidOperationException($"Invalid attribute type \"{type}\".");
            //    }
            //}

            return Task.FromResult("Aggregation started");
        }
    }
}
