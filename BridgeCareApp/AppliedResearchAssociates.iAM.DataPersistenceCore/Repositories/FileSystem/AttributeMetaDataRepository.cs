using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Attributes;
using Newtonsoft.Json;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem
{
    public class AttributeMetaDataRepository : IAttributeMetaDataRepository
    {
         public List<Attribute> GetAllAttributes(Guid dataSourceId)
        {
            // set the attribute meta data json file path
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty,
                "MetaData//AttributeMetaData", "metaData.json");

            // check that the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} does not exist");
            }

            // get the raw json from the file
            var rawAttributes = File.ReadAllText(filePath);

            // convert the json string into attribute meta data models
            var attributeMetaData = JsonConvert
                .DeserializeAnonymousType(rawAttributes, new { AttributeMetaData = default(List<AttributeMetaDatum>) })
                .AttributeMetaData;

            // Check to see if the GUIDs in the meta data are blank. A blank GUID requires that the
            // attribute has never been assigned in a network previously.
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

                // update the json file with the guid changes
                using var writer = new StreamWriter(filePath);
                writer.Write(JsonConvert.SerializeObject(new { AttributeMetaData = attributeMetaData }));
            }

            // convert meta data into attribute domain models
            var attributes = attributeMetaData.Select(a => AttributeFactory.Create(a, dataSourceId)).ToList();

            return attributes;
        }
    }
}
