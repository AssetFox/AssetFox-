using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistence.Models;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.FileSystem
{
    public class SegmentationRepository : GenericFileSystemRepository<SegmentationRule>
    {
        public override IEnumerable<SegmentationRule> All()
        {
            var folderPath = $"MetaData//Segment";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPath, "sectionAttribute.json");
            var segmentMetaData = new List<SegmentationRule>();
            if (File.Exists(filePath))
            {
                var rawAttributes = File.ReadAllText(filePath);
                segmentMetaData = JsonConvert.DeserializeAnonymousType(rawAttributes, new { SegmentData = default(List<SegmentationRule>) }).SegmentData;
            }
            return segmentMetaData;
        }
    }
}
