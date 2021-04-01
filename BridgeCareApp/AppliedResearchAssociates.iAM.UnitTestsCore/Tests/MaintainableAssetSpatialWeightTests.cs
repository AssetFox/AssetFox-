using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using Xunit;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class MaintainableAssetSpatialWeightTests
    {
        private static readonly Guid EquationId = Guid.Parse("5563b7f9-d42c-4bfa-8504-eb57a68e427a");

        private readonly TestHelper _testHelper;

        public MaintainableAssetSpatialWeightTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            DeckAreaAttribute = _testHelper.UnitOfWork.Context.Attribute
                .Single(_ => _.Name.ToLower().Contains("deck_area")).ToDomain();
        }

        private Attribute DeckAreaAttribute { get; }

        [Fact]
        public void ShouldSetMaintainableAssetSpatialWeightingBasedOnBenefitQuantifier()
        {
            try
            {
                // Arrange
                var location = new SectionLocation(Guid.NewGuid(), "12");
                var maintainableAsset = new MaintainableAsset(Guid.NewGuid(), _testHelper.TestNetwork.Id, location);
                var attributeDatum =
                    new AttributeDatum<double>(Guid.NewGuid(), DeckAreaAttribute, 1000, location, DateTime.Now);
                maintainableAsset.AssignAttributeData(new List<IAttributeDatum>{attributeDatum});

                // Act
                maintainableAsset.AssignSpatialWeighting(DeckAreaAttribute.Name);

                // Assert
                Assert.NotNull(maintainableAsset.SpatialWeighting);
                Assert.Equal(attributeDatum.Value, maintainableAsset.SpatialWeighting.Area);
            }
            finally
            {
                _testHelper.CleanUp();
            }
        }
    }
}
