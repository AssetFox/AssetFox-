using System;
using System.Collections.Generic;
using System.IO;
using AppliedResearchAssociates.iAM.DataMiner;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem
{
    public class SegmentationMetaDataRepository : FileSystemRepository<AttributeMetaDatum>
    {
        public override IEnumerable<AttributeMetaDatum> All()
        {
            var folderPath = $"MetaData//Segment";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPath, "segmentationMetaData.json");
            var segmentMetaData = new List<AttributeMetaDatum>();
            if (File.Exists(filePath))
            {
                var segmentationRulesJsonText = File.ReadAllText(filePath);
                segmentMetaData.Add(JsonConvert.DeserializeAnonymousType(segmentationRulesJsonText,
                new { AttributeMetaDatum = default(AttributeMetaDatum) }).AttributeMetaDatum);
            }
            return segmentMetaData;
        }
    }
}
