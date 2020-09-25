using System;
using System.Collections.Generic;
using System.IO;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Models;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem
{
    public class AttributeRepository : GenericFileSystemRepository<AttributeMetaDatum>
    {
        public AttributeRepository()
        {
        }

        public override IEnumerable<AttributeMetaDatum> All()
        {
            var folderPath = $"MetaData//AttributeMetaData";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPath, "metaData.json");
            var attributeMetaData = new List<AttributeMetaDatum>();
            if (File.Exists(filePath))
            {
                var rawAttributes = File.ReadAllText(filePath);
                attributeMetaData = JsonConvert.DeserializeAnonymousType(rawAttributes, new { AttributeMetaData = default(List<AttributeMetaDatum>) }).AttributeMetaData;
            }
            return attributeMetaData;
        }
    }
}
