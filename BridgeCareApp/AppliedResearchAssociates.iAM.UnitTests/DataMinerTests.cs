﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using ExecutableForProtptype;
using Newtonsoft.Json;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace AppliedResearchAssociates.iAM.UnitTests
{
    [TestFixture]
    public class DataMinerTests
    {
        public List<AttributeDatum<double>> Attribute1 { get; } = new List<AttributeDatum<double>>()
        {
            new AttributeDatum<double>(iAMConfiguration.C, 100, new LinearLocation(new DirectionalRoute("B", Direction.N),"unique identifier", 0, 1), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 200, new LinearLocation(new DirectionalRoute("B", Direction.N),"unique identifier", 1, 2), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 300, new LinearLocation(new DirectionalRoute("B", Direction.N),"unique identifier", 2, 3), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 400, new LinearLocation(new DirectionalRoute("B", Direction.N),"unique identifier", 3, 4), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 500, new LinearLocation(new DirectionalRoute("B", Direction.N),"unique identifier", 4, 5), DateTime.Now),

            new AttributeDatum<double>(iAMConfiguration.C, 101, new LinearLocation(new DirectionalRoute("B", Direction.S),"unique identifier", 4, 5), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 201, new LinearLocation(new DirectionalRoute("B", Direction.S),"unique identifier", 3, 4), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 301, new LinearLocation(new DirectionalRoute("B", Direction.S),"unique identifier", 2, 3), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 401, new LinearLocation(new DirectionalRoute("B", Direction.S),"unique identifier", 1, 2), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 501, new LinearLocation(new DirectionalRoute("B", Direction.S),"unique identifier", 0, 1), DateTime.Now),
        };

        public List<AttributeDatum<string>> Attribute2 { get; } = new List<AttributeDatum<string>>()
        {
            new AttributeDatum<string>(iAMConfiguration.B, "ALABAMA", new LinearLocation(new DirectionalRoute("B", Direction.S),"unique identifier", 0, 1), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "ALASKA", new LinearLocation(new DirectionalRoute("B", Direction.S),"unique identifier", 1, 2), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "ARIZONA", new LinearLocation(new DirectionalRoute("B", Direction.S),"unique identifier", 2, 3), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "ARKANSAS", new LinearLocation(new DirectionalRoute("B", Direction.S),"unique identifier", 3, 4), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "CALIFORNIA", new LinearLocation(new DirectionalRoute("B", Direction.S),"unique identifier", 4, 5), DateTime.Now),

            new AttributeDatum<string>(iAMConfiguration.B, "COLORADO", new SectionLocation("B-0-1"), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "CONNECTICUT", new SectionLocation("B-1-2"), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "DELEWARE", new SectionLocation("B-2-3"), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "FLORIDA", new SectionLocation("B-3-4"), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "GEORGIA", new SectionLocation("B-4-5"), DateTime.Now)
        };

        public static SqlAttributeConnection SQLConnection = new SqlAttributeConnection(TestDataForAttribute.Connectionstring, "select * from ADT");
        public static LinearLocation LinearLocation = new LinearLocation(new SimpleRoute("Test simple route"), "unique identifier", 0, 10);

        public static AttributeDatum<double> NumericAttributeDatum = new AttributeDatum<double>(
            new NumericAttribute("ADT", SQLConnection, 10, 100, 1), 5, LinearLocation, DateTime.Now);

        public DataMinerTests()
        {
        }

        [Test]
        public void CreateAttributes()
        {
            var rawAttributes = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\metaData.json");
            var attributeMetaData = JsonConvert.DeserializeAnonymousType(rawAttributes, new { AttributeMetaData = default(List<AttributeMetaDatum>) }).AttributeMetaData;

            foreach (var item in attributeMetaData)
            {
                Assert.That(item.DataRetrievalCommand, Is.Not.Null);
                Assert.That(item.DataType, Is.Not.Null);
                if (item.DataType.ToLower().Equals("number"))
                {
                    if (item.Location.ToLower().Equals("linear"))
                    {
                        // Arrange/ Act
                        var numericAttributeData = new NumericAttributeDataCreator();
                        var result = numericAttributeData.GetNumericAttributeDatum(item);
                        foreach (var data in result)
                        { // make sure that the properties are not null. We should not have any currupt data
                            Assert.That(data.Location, Is.Not.Null);
                            Assert.AreEqual(data.Attribute.Name, NumericAttributeDatum.Attribute.Name);
                            Assert.That(data.Attribute.Name, Is.Not.Null);
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (item.Location.ToLower().Equals("section"))
                    {

                    }
                    else
                    {

                    }
                }
            }
        }

        [Test]
        public void GetNumericDataSqlConnection()
        {
            // Arrange
            var sqlAttributeConnection = new SqlAttributeConnection(TestDataForAttribute.Connectionstring, "select * from ADT");
            // Act
            var output = sqlAttributeConnection.GetData<double>();

            // Assert
            foreach (var item in output)
            {
                Assert.That(item.value, Is.Not.Null); //convert to double shouldn't throw error
                Assert.That(item.value, Is.TypeOf<double>());
            }
        }

        [Test]
        public void GetTextDataSqlConnection()
        {
            //// Arrange
            //var sqlAttributeConnection = new SqlAttributeConnection(TestDataForAttribute.Connectionstring);
            //// Act
            //var output = sqlAttributeConnection.GetData<string>("ADT");

            //// Assert
            //foreach (var item in output)
            //{
            //    Assert.That(item.value, Is.TypeOf<string>());
            //}
        }

        [Test]
        public void CreateNumericAttributeForLinearLocation()
        {
            // Arrange / Act
            var output = AttributeDatumBuilder<double>.CreateAttributeData(iAMConfiguration.C, TestDataForAttribute.LinearLocationForNumberAttribute);

            foreach (var item in output)
            {
                Assert.That(item.Value, Is.TypeOf<double>());
                Assert.That(item.Value, Is.EqualTo(TestDataForAttribute.NumericAttributeDataLinearLocation.Value));
            }
        }

        [Test]
        public void CreateTextAttributeForSectionLocation()
        {
            // Arrange / Act
            var output = AttributeDatumBuilder<string>.CreateAttributeData(iAMConfiguration.B, TestDataForAttribute.SectionLocationForTextAttribute);

            foreach (var item in output)
            {
                Assert.That(item.Value, Is.TypeOf<string>());
                Assert.That(item.Value, Is.EqualTo(TestDataForAttribute.TextAttributeDataSectionLocOutput.Value));
            }
        }

        [Test]
        public void CreateTextAttributeForLinearLocation()
        {
            // Arrange / Act
            var output = AttributeDatumBuilder<string>.CreateAttributeData(iAMConfiguration.B, TestDataForAttribute.LinearLocationForTextAttribute);

            foreach (var item in output)
            {
                Assert.That(item.Value, Is.TypeOf<string>());
                Assert.That(item.Value, Is.EqualTo(TestDataForAttribute.TextAttributeDataLinearLocOutput.Value));
            }
        }

        [Test]
        public void CreateNumericAttributeForSectionLocation()
        {
            // Arrange / Act
            var output = AttributeDatumBuilder<double>.CreateAttributeData(iAMConfiguration.C, TestDataForAttribute.SectionLocationForNumberAttribute);

            foreach (var item in output)
            {
                Assert.That(item.Value, Is.TypeOf<double>());
                Assert.That(item.Value, Is.EqualTo(TestDataForAttribute.NumericAttributeSectionLocOutput.Value));
            }
        }
    }
}
