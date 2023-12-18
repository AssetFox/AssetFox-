using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Mappers;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class AttributeDatumTestSetup
    {
        private static Guid AssignAttributeDatum<T>(
            AttributeDTO attributeDto,
            MaintainableAsset asset,
            T datumValue,
            string locationName = "location"
            )
        {
            var assets = new List<MaintainableAsset> { asset };
            var datumId = Guid.NewGuid();
            var location = Locations.Section(locationName);
            var domainAttribute = AttributeDtoDomainMapper.ToDomain(attributeDto, "");
            var datum = new AttributeDatum<T>(datumId, domainAttribute, datumValue, location, DateTime.Now);
            var attributeDtos = new List<AttributeDTO> { attributeDto };
            asset.AssignedData.Add(datum);
            TestHelper.UnitOfWork.AttributeDatumRepo.AddAssignedData(assets, attributeDtos);
            return datumId;
        }

        public static Guid AssignStringAttributeDatum(
            AttributeDTO attributeDto,
            MaintainableAsset asset,
            string datumValue = "where",
            string locationName = "location"
            )
        {
            var guid = AssignAttributeDatum(
                attributeDto, asset, datumValue, locationName);
            return guid;
        }

        public static Guid AssignDoubleAttributeDatum(
            AttributeDTO attributeDto,
            MaintainableAsset asset,
            double datumValue,
            string locationName = "location"
            )
        {
            var guid = AssignAttributeDatum(
                attributeDto, asset, datumValue, locationName);
            return guid;
        }
    }
}
