using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using DataAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes;
using AppliedResearchAssociates.iAM.Data.Mappers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories
{
    public class AttributeRepositoryTests
    {
        private IAttributeRepository attributeRepository => TestHelper.UnitOfWork.AttributeRepo;

        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            CalculatedAttributeTestSetup.CreateCalculatedAttributeLibrary(TestHelper.UnitOfWork);
        }

        [Fact]
        public async Task AddNumericAttribute_Does()
        {
            Setup();
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesAfter = await repo.GetAttributesAsync();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(1, attributeAfter.Minimum);
            Assert.Equal(3, attributeAfter.Maximum);
            Assert.Equal("2", attributeAfter.DefaultValue);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task AddTextAttribute_Does()
        {
            Setup();
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Text();
            var attributes = new List<DataAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesAfter = await repo.GetAttributesAsync();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Null(attributeAfter.Minimum);
            Assert.Null(attributeAfter.Maximum);
            Assert.Equal("defaultValue", attributeAfter.DefaultValue);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task AttributeInDb_UpdateAllowedFields_Does()
        {
            Setup();
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesBefore = await repo.GetAttributesAsync();
            var attributeBefore = attributesBefore.Single(a => a.Id == attribute.Id);
            var updateAttribute = new NumericAttribute(
                20, 100, 10, attribute.Id, attribute.Name, "AVERAGE",
                "updatedCommand", Data.ConnectionType.MSSQL, "connectionString",
                !attribute.IsCalculated, !attribute.IsAscending, Guid.Empty);
            var updateAttributes = new List<DataAttribute> { updateAttribute };
            repo.UpsertAttributes(updateAttributes);
            var attributesAfter = await repo.GetAttributesAsync();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            ObjectAssertions.Equivalent(attributeBefore, attributeAfter);
        }

        [Fact]
        public async Task NumericAttributeInDb_UpdateNameOnly_ThrowsWithoutUpdating()
        {
            Setup();
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesBefore = await repo.GetAttributesAsync();
            var attributeBefore = attributesBefore.Single(a => a.Id == attribute.Id);
            var updateAttribute = AttributeTestSetup.Numeric(attribute.Id, "updated name should fail");
            var updateAttributes = new List<DataAttribute> { updateAttribute };
            var exception = Assert.Throws<InvalidAttributeUpsertException>(() => repo.UpsertAttributes(updateAttributes));
            Assert.Contains(AttributeUpdateValidityChecker.NameChangeNotAllowed, exception.Message);
            var attributesAfter = await repo.GetAttributesAsync();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            ObjectAssertions.Equivalent(attributeBefore, attributeAfter);
        }

        [Fact]
        public async Task NumericAttributeInDb_UpdateFieldsThatAreNotAllowedToChange_ThrowsWithoutUpdating()
        {
            Setup();
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesBefore = await repo.GetAttributesAsync();
            var attributeBefore = attributesBefore.Single(a => a.Id == attribute.Id);
            var updateAttribute = new NumericAttribute(222, 1000, 123, attribute.Id, "this should kill the update", "Last", "update command", Data.ConnectionType.MSSQL, "connectionString", !attribute.IsCalculated, !attribute.IsAscending, Guid.Empty);
            Assert.Throws<InvalidAttributeUpsertException>(() => repo.UpsertAttributes(updateAttribute));
            var attributesAfter = await repo.GetAttributesAsync();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            ObjectAssertions.Equivalent(attributeBefore, attributeAfter);
        }

        [Fact]
        public async Task AddInvalidAttribute_Fails()
        {
            Setup();
            // rewrite to use the dto. Conversion of the dto to a NumericAttribute object should fail.
            var repo = attributeRepository;
            var randomName = RandomStrings.Length11();
            var attributeId = Guid.NewGuid();
            var attributeDto = new AttributeDTO
            {
                Id = attributeId,
                Name = randomName,
                Minimum = 0,
                Maximum = 1000,
                DefaultValue = "100",
                AggregationRuleType = "Invalid rule",
                Command = "Command",
                Type = "NUMBER",
                IsAscending = false,
                IsCalculated = false,
            };
            var invalidAttributeList = new List<AttributeDTO> { attributeDto };
            Assert.Throws<InvalidOperationException>(() => repo.UpsertAttributes(invalidAttributeList));
            var attributesAfter = await repo.GetAttributesAsync();
            var addedAttribute = attributesAfter.SingleOrDefault(a => a.Id == attributeId);
            Assert.Null(addedAttribute);
        }

        [Fact]
        public async Task AttributeInDb_CreateNewAttributeWithSameName_Throws()
        {
            Setup();
            var repo = attributeRepository;
            var randomName = AttributeTestSetup.ValidAttributeName();
            var attribute = AttributeTestSetup.Numeric(name: randomName); ;
            repo.UpsertAttributes(attribute);
            var attributesBefore = await repo.GetAttributesAsync();
            var attributeBefore = attributesBefore.Single(a => a.Id == attribute.Id);
            var attribute2 = AttributeTestSetup.Numeric(name: randomName);
            Assert.ThrowsAny<Exception>(() => repo.UpsertAttributes(attribute2));
            var attributesAfter = await repo.GetAttributesAsync();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(attributesBefore.Count, attributesAfter.Count);
            var addedAttribute2 = attributesAfter.SingleOrDefault(a => a.Id == attribute2.Id);
            Assert.Null(addedAttribute2);
            ObjectAssertions.Equivalent(attributeBefore, attributeAfter);
        }

        [Fact]
        public async Task CreateTwoAttributes_OneValidOneNot_ThrowsWithoutCreatingEither()
        {
            Setup();
            var repo = attributeRepository;
            var randomName = RandomStrings.Length11();
            var attributeId = Guid.NewGuid();
            var dataSourceDto = DataSourceTestSetup.DtoForExcelDataSourceInDb(TestHelper.UnitOfWork);
            var attributeDto = new AttributeDTO
            {
                Id = attributeId,
                Name = randomName,
                Minimum = 0,
                Maximum = 1000,
                DefaultValue = "100",
                AggregationRuleType = "Invalid rule",
                Command = "Command",
                Type = "NUMBER",
                IsAscending = false,
                IsCalculated = false,
            };
            var validAttribute = AttributeTestSetup.NumericDto(dataSourceDto);

            var invalidAttributeList = new List<AttributeDTO> { attributeDto };
            Assert.Throws<InvalidOperationException>(() => repo.UpsertAttributes(invalidAttributeList));
            var attributesAfter = await repo.GetAttributesAsync();
            var addedAttribute = attributesAfter.FirstOrDefault(a =>
               a.Id == attributeId
               || a.Id == validAttribute.Id);
            Assert.Null(addedAttribute);
        }

        [Fact]
        public void AttributeInDb_GetSingleById_Does()
        {
            Setup();
            var repo = attributeRepository;
            var randomName = AttributeTestSetup.ValidAttributeName();
            var attribute = AttributeTestSetup.Numeric(name: randomName);
            repo.UpsertAttributes(attribute);

            var attributeAfter = repo.GetSingleById(attribute.Id);

            Assert.NotNull(attributeAfter);
        }

        [Fact]
        public void AttributeNotInDb_GetSingleById_ReturnsNull()
        {
            Setup();
            var repo = attributeRepository;
            var attribute = repo.GetSingleById(Guid.NewGuid());
            Assert.Null(attribute);
        }

        [Fact]
        public void AddAttributeWithDataSourceWithConnectionString_LoadFromDb_ConnectionStringIsThere()
        {
            Setup();
            var dataSourceId = Guid.NewGuid();
            var randomName = RandomStrings.Length11();
            var dataSource = new SQLDataSourceDTO
            {
                ConnectionString = "data source=Test;initial catalog=TestDB;persist security info=True;user id=TestId;password=TestPassword;MultipleActiveResultSets=True;App=EntityFramework",
                Id = dataSourceId,
                Name = randomName,
            };
            var dataSourceRepo = TestHelper.UnitOfWork.DataSourceRepo;
            dataSourceRepo.UpsertDatasource(dataSource);
            var attributeId = Guid.NewGuid();
            var attributeName = RandomStrings.WithPrefix("AttributeName");
            var attributeDto = new AttributeDTO
            {
                Id = attributeId,
                AggregationRuleType = TextAttributeAggregationRules.Predominant,
                Command = "Command",
                DataSource = dataSource,
                Name = attributeName,
                Type = "STRING"//AppliedResearchAssociates.iAM.AttributeTypeNames.String
            };
            TestHelper.UnitOfWork.AttributeRepo.UpsertAttributes(attributeDto);
            var attributeAfter = TestHelper.UnitOfWork.AttributeRepo.GetSingleById(attributeId);
            var sqlDataSourceAfter = attributeAfter.DataSource as SQLDataSourceDTO;
            Assert.Equal("data source=Test;initial catalog=TestDB;persist security info=True;user id=TestId;password=TestPassword;MultipleActiveResultSets=True;App=EntityFramework", sqlDataSourceAfter.ConnectionString);
            var domainAttributeAfter = AttributeDtoDomainMapper.ToDomain(attributeAfter, TestHelper.UnitOfWork.EncryptionKey);
            Assert.Equal("data source=Test;initial catalog=TestDB;persist security info=True;user id=TestId;password=TestPassword;MultipleActiveResultSets=True;App=EntityFramework", domainAttributeAfter.ConnectionString);
        }

        [Fact]
        public void GetAttributeIdsInNetwork_NetworkInDbWithAttribute_GetsAttributeId()
        {
            var networkId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var locationIdentifier = RandomStrings.WithPrefix("Location");
            var location = Locations.Section(locationIdentifier);
            var maintainableAsset = new MaintainableAsset(assetId, networkId, location, "[Deck_Area]");
            var maintainableAssets = new List<MaintainableAsset> { maintainableAsset };
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, maintainableAssets, networkId);
            var attributeId = Guid.NewGuid();
            var attribute = AttributeTestSetup.Numeric(attributeId);
            TestHelper.UnitOfWork.AttributeRepo.UpsertAttributes(attribute);
            var aggregatedDatum = (2022, 123.4);
            var triplet = (attribute, aggregatedDatum);
            var triplets = new List<(DataAttribute, (int, double))> { triplet };
            var aggregatedResultId = Guid.NewGuid();
            var aggregatedResult = new AggregatedResult<double>(aggregatedResultId, maintainableAsset, triplets);
            var aggregatedResults = new List<IAggregatedResult> { aggregatedResult };
            TestHelper.UnitOfWork.AggregatedResultRepo.AddAggregatedResults(aggregatedResults);

            var attributeIds = TestHelper.UnitOfWork.AttributeRepo.GetAttributeIdsInNetwork(networkId);

            var actual = attributeIds.Single();
            Assert.Equal(actual, attributeId);
        }

        [Fact]
        public void GetAttributeIdsInNetwork_NetworkNotInTable_Throws()
        {

            var networkId = Guid.NewGuid();

            var exception = Assert.Throws<RowNotInTableException>(() => TestHelper.UnitOfWork.AttributeRepo.GetAttributeIdsInNetwork(networkId));

        }

        [Fact]
        public async Task GetCalculatedAttributes_CalculatedAttributeInDb_Gets()
        {
            var attributeId = Guid.NewGuid();
            var attribute = AttributeTestSetup.Text(attributeId, calculated: true);
            TestHelper.UnitOfWork.AttributeRepo.UpsertAttributes(attribute);

            var calculatedAttributes = await TestHelper.UnitOfWork.AttributeRepo.CalculatedAttributes();

            var actual = calculatedAttributes.Single(a => a.Id == attribute.Id);
            Assert.NotNull(actual);
        }
    }
}
