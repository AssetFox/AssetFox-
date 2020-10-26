using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Newtonsoft.Json;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

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
            var attributeMetaData = JsonConvert
                .DeserializeAnonymousType(rawAttributes, new { AttributeMetaData = default(List<AttributeMetaDatum>) })
                .AttributeMetaData;

            // Check to see if the GUIDs in the meta data repo are blank. A blank GUID requires
            // that the attribute has never been assigned in a network previously.
            if (attributeMetaData.Any(_ => Guid.Empty == _.Id))
            {
                // give new meta data a guid
                attributeMetaData = attributeMetaData.Select(_ =>
                {
                    if (Guid.Empty == _.Id)
                    {
                        _.Id = Guid.NewGuid();
                    }

                    return _;
                }).ToList();

                using var writer = new StreamWriter(filePath);
                writer.Write(JsonConvert.SerializeObject(new { AttributeMetaData = attributeMetaData }));
            }

            return attributeMetaData;
        }
    }
}
