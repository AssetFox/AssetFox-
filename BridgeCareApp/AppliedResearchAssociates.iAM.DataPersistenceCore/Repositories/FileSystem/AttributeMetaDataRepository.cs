using System.Collections.Generic;
using System.IO;
using AppliedResearchAssociates.iAM.DataMiner;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem
{
    public class AttributeMetaDataRepository : IAttributeMetaDataRepository
    {
        public List<AttributeMetaDatum> All(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} does not exist");
            }

            var rawAttributes = File.ReadAllText(filePath);
            return JsonConvert
                .DeserializeAnonymousType(rawAttributes, new {AttributeMetaData = default(List<AttributeMetaDatum>)})
                .AttributeMetaData;
        }

        public void UpdateAll(string filePath, List<AttributeMetaDatum> attributeMetaData)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} does not exist");
            }

            using var writer = new StreamWriter(filePath);
            writer.Write(JsonConvert.SerializeObject(attributeMetaData));
        }
    }
}
