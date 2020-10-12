using System;
using System.Collections.Generic;
using System.IO;
using AppliedResearchAssociates.iAM.DataMiner;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem
{
    public class NetworkDefinitionMetaDataRepository : FileSystemRepository<AttributeMetaDatum>
    {
        public override IEnumerable<AttributeMetaDatum> All()
        {
            var folderPath = $"MetaData//NetworkDefinitionRules";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPath, "networkDefinitionRules.json");
            var maintainableAssetMetaData = new List<AttributeMetaDatum>();
            if (File.Exists(filePath))
            {
                var jsonText = File.ReadAllText(filePath);
                maintainableAssetMetaData.Add(JsonConvert.DeserializeAnonymousType(jsonText,
                new { AttributeMetaDatum = default(AttributeMetaDatum) }).AttributeMetaDatum);
            }
            return maintainableAssetMetaData;
        }
    }
}
