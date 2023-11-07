using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Mappers;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using DataAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using DataTextAttribute = AppliedResearchAssociates.iAM.Data.Attributes.TextAttribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.AttributeDatum
{
    public class AttributeDatumRepositoryTests
    {
        [Fact]
        public void GetAllInNetwork_AssetInNetworkWithAttributeDatum_Gets()
        {
            var networkId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var keyAttributeId = Guid.NewGuid();
            var keyAttributeName = RandomStrings.WithPrefix("KeyAttribute");
            var attributeDto = AttributeTestSetup.CreateSingleTextAttribute(TestHelper.UnitOfWork, keyAttributeId, keyAttributeName, ConnectionType.EXCEL, "location");
            var asset = MaintainableAssets.InNetwork(networkId, keyAttributeName, assetId);
            var assets = new List<MaintainableAsset> { asset };
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, assets, networkId, keyAttributeId);
            var explorer = TestHelper.UnitOfWork.AttributeRepo.GetExplorer();
            TestHelper.UnitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            var assetIds = new List<Guid> { assetId };
            var attributeIds = new List<Guid> { keyAttributeId };
            var attributeDtos = new List<AttributeDTO> { attributeDto };
            var datumId = Guid.NewGuid();
            var location = Locations.Section("location");
            var domainAttribute = AttributeDtoDomainMapper.ToDomain(attributeDto, "");
            var datum = new AttributeDatum<string>(datumId, domainAttribute, "where", location, DateTime.Now);
            asset.AssignedData.Add(datum);
            TestHelper.UnitOfWork.AttributeDatumRepo.AddAssignedData(assets, attributeDtos);

            var attributeData = TestHelper.UnitOfWork.AttributeDatumRepo.GetAllInNetwork(
                assetIds, attributeIds);

            var attributeDatum = attributeData.Single();
            var expectedAttributeDatum = new AttributeDatumDTO
            {
                Id = datumId,
                MaintainableAssetId = assetId,
                TextValue = "where",
                Attribute = keyAttributeName,
            };
            ObjectAssertions.Equivalent(expectedAttributeDatum, attributeDatum);
        }
    }
}
