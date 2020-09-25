using System;
using System.Collections.Generic;
using System.IO;
using AppliedResearchAssociates.iAM.DataMiner;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem
{
    public class SegmentationRepository : FileSystemRepository<AttributeMetaDatum>
    {
        public override IEnumerable<AttributeMetaDatum> All()
        {
            var folderPath = $"MetaData//Segment";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPath, "sectionAttribute.json");
            var segmentMetaData = new List<AttributeMetaDatum>();
            if (File.Exists(filePath))
            {
                var rawAttributes = File.ReadAllText(filePath);
                segmentMetaData = JsonConvert.DeserializeAnonymousType(rawAttributes, new { SegmentData = default(List<AttributeMetaDatum>) }).SegmentData;
            }
            return segmentMetaData;
        }
    }
}
