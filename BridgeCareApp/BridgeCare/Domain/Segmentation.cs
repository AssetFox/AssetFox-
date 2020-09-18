using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistence.Models;
using AppliedResearchAssociates.iAM.DataPersistence.Repositories;
using AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL;

namespace BridgeCare.Domain
{
    public class Segmentation
    {
        private readonly IRepository<SegmentationRule> SegmentationRepository;

        public Segmentation(IRepository<SegmentationRule> segmentationRepository)
        {
            SegmentationRepository = segmentationRepository;
        }

        public void Run()
        {
            var segmentationRules = SegmentationRepository.All();

            foreach (var rule in segmentationRules) //TODO: Use network id to segment only the selected network
            {
                var type = rule.AttributeMetaDatum.DataType;
                var name = rule.AttributeMetaDatum.AttributeName;

                switch (type.ToUpper())
                {
                case NUMERIC_ATTRIBUTE:
                    var numericAttribute = new NumericAttribute(name, )
                    var numericAttributeData = AttributeDatumBuilder<double>.CreateAttributeData(numericAttribute, rawData);

                    // Segmentation

                    var network = Segmenter.CreateNetworkFromAttributeDataRecords<double>(numericAttributeData, guid);
                    networks.Add(network.GUID, network);
                    break;
                case STRING_ATTRIBUTE_TYPE_NAME:
                    var rawTextData = dataRetrival.GetRawData<string>();
                    var textAttribute = dataRetrival.GetTextAttribute(name, section.AttributeMetaDatum.DefaultValue);

                    var textAttributeData = AttributeDatumBuilder<string>.CreateAttributeData(textAttribute, rawTextData);
                    network = Segmenter.CreateNetworkFromAttributeDataRecords<double>(textAttributeData);
                    networks.Add(network.GUID, network);
                    break;
                }
            }
        }
    }
}
