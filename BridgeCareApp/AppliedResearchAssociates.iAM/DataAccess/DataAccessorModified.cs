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
        public void Test()
        {
            var attributeJsonText = File.ReadAllText("attributeMetaData.json");
            var attributeMetaData = JsonConvert.DeserializeAnonymousType(attributeJsonText, new { AttributeMetaData = default(List<AttributeMetaDatum>) }).AttributeMetaData;

            var segmentationRulesJsonText = File.ReadAllText("segmentationMetaData.json");
            var segmentationRulesMetaData = JsonConvert.DeserializeAnonymousType(segmentationRulesJsonText, new { AttributeMetaData = default(AttributeMetaDatum) }).AttributeMetaData;

            switch(segmentationRulesMetaData.Type)
            {
                case "NUMERIC":
                {
                    if (!double.TryParse(segmentationRulesMetaData.DefaultValue, out double defaultValue))
                    {
                        throw new InvalidCastException($"Numeric attribute {segmentationRulesMetaData.Name} does not have a valid numeric default value. Please check the value in the configuration file and try again.");
                    }

                    var attribute = new NumericAttribute(
                            segmentationRulesMetaData.Name,
                            AttributeConnectionBuilder.Create(segmentationRulesMetaData.ConnectionType, segmentationRulesMetaData.ConnectionString, segmentationRulesMetaData.Command),
                            defaultValue,
                            segmentationRulesMetaData.Maximum,
                            segmentationRulesMetaData.Minimum);

                    var attributeData = attribute.Connection.GetData<double>();
                    GetNetworkFromSingleAttribute<double>(attributeData);

                    // Save to the database

                    break;
                }
                case "TEXT":
                {
                    var attribute =
                        new DataMiner.Attributes.TextAttribute(
                            segmentationRulesMetaData.Name,
                            AttributeConnectionBuilder.Create(
                                segmentationRulesMetaData.ConnectionType,
                                segmentationRulesMetaData.ConnectionString,
                                segmentationRulesMetaData.Command),
                            segmentationRulesMetaData.DefaultValue);

                    var attributeData = attribute.Connection.GetData<double>();
                    GetNetworkFromSingleAttribute<double>(attributeData);

                    // Save to the database

                    break;
                }
            }
        }

        private AppliedResearchAssociates.iAM.Segmentation.Network GetNetworkFromSingleAttribute<T>(IEnumerable<IAttributeDatum> attributeData)
        {
            return Segmenter.CreateNetworkFromAttributeDataRecords<T>(attributeData);
        }
    }
}
