﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;
using DataAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories
{
    public class AttributeRepositoryTests
    {
        private readonly TestHelper _testHelper;
        private IAttributeRepository attributeRepository => _testHelper.UnitOfWork.AttributeRepo;

        public AttributeRepositoryTests()
        {
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.CreateCalculatedAttributeLibrary();
            }
        }

        [Fact]
        public async Task AddNumericAttribute_Does()
        {
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesAfter = await repo.Attributes();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(1, attributeAfter.Minimum);
            Assert.Equal(3, attributeAfter.Maximum);
            Assert.Equal("2", attributeAfter.DefaultValue);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task AddTextAttribute_Does()
        {
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Text();
            var attributes = new List<DataAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var attributesAfter = await repo.Attributes();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Null(attributeAfter.Minimum);
            Assert.Null(attributeAfter.Maximum);
            Assert.Equal("defaultValue", attributeAfter.DefaultValue);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task AttributeInDb_UpdateAllowedFields_Does()
        {
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var updateAttribute = new NumericAttribute(
                20, 100, 10, attribute.Id, attribute.Name, "AVERAGE",
                "updatedCommand", Data.ConnectionType.MSSQL, "connectionString",
                !attribute.IsCalculated, !attribute.IsAscending);
            var updateAttributes = new List<DataAttribute> { updateAttribute };
            repo.UpsertAttributes(updateAttributes);
            var attributesAfter = await repo.Attributes();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(10, attributeAfter.Minimum);
            Assert.Equal(100, attributeAfter.Maximum);
            Assert.Equal("20", attributeAfter.DefaultValue);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task NumericAttributeInDb_UpdateNameOnly_NameIsNotChanged()
        {
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var updateAttribute = AttributeTestSetup.Numeric(attribute.Id, "updated name should fail");
            var updateAttributes = new List<DataAttribute> { updateAttribute };
            repo.UpsertAttributes(updateAttributes);
            var attributesAfter = await repo.Attributes();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task NumericAttributeInDb_UpdateManyFields_NotChanged()
        {
            var repo = attributeRepository;
            var attribute = AttributeTestSetup.Numeric();
            var attributes = new List<DataAttribute> { attribute };
            repo.UpsertAttributes(attributes);
            var updateAttribute = new NumericAttribute(222, 1000, 123, attribute.Id, "this should kill the update", "update rule type", "update command", Data.ConnectionType.MSSQL, "connectionString", !attribute.IsCalculated, !attribute.IsAscending);
            repo.UpsertAttributes(updateAttribute);
            var attributesAfter = await repo.Attributes();
            var attributeAfter = attributesAfter.Single(a => a.Id == attribute.Id);
            Assert.Equal(attribute.Name, attributeAfter.Name);
        }

        [Fact]
        public async Task AddInvalidAttribute_Fails()
        {
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
            Assert.ThrowsAny<Exception>(() => repo.UpsertAttributes(invalidAttributeList));
            var attributesAfter = await repo.Attributes();
            var addedAttribute = attributesAfter.SingleOrDefault(a => a.Id == attributeId);
            Assert.Null(addedAttribute);
        }

        public async Task AttributeInDb_CreateNewAttributeWithSameName_Fails()
        {
            var repo = attributeRepository;
            var randomName = RandomStrings.Length11();
            var attribute = AttributeTestSetup.Numeric(null, randomName);
            repo.UpsertAttributes(attribute);
            var attributesBefore = await repo.Attributes();
            var attribute2 = AttributeTestSetup.Numeric(null, randomName);
            repo.UpsertAttributes(attribute2);
            var attributesAfter = await repo.Attributes();
            Assert.Equal(attributesBefore.Count, attributesAfter.Count);
            var addedAttribute2 = attributesAfter.SingleOrDefault(a => a.Id == attribute2.Id);
            Assert.Null(addedAttribute2);
        }
    }
}
