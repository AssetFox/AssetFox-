using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attribute_Tests
{
    public class AADDTTest
    {
        private AttributeMetaDatum AADDTMetaDatum = new AttributeMetaDatum()
        {
            Name = "ADTTOTAL",

            Id = Guid.Parse("6a473634-ce64-4cae-acda-7306a2495454"),

            DefaultValue = "1000",

            Minimum = 0.0,

            Maximum = 0.0,

            ConnectionString = "data source=localhost;initial catalog=DbBackup;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework",

            DataSource = "MSSQL",

            Type = "NUMBER",

            Location = "section",

            Command = "SELECT CAST(BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(REPLACE(ADTTOTAL, ',', '') AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (ISNUMERIC(ADTTOTAL) = 1)",

            AggregationRule = "AVERAGE",

            IsCalculated = false,

            IsAscending = true
        };
        private static readonly Guid NetworkId = Guid.Parse("D7B54881-DD44-4F93-8250-3D4A630A4D3B"); //7f4ea3ba-6082-4e1e-91a4-b80578aeb0ed
        //D7B54881-DD44-4F93-8250-3D4A630A4D3B

        private static readonly Guid SimulationId = Guid.Parse("416ad546-0796-4889-9db4-9c11bbd6c50d");
        private readonly TestHelper _testHelper;

        public readonly IAMContext DbContext;

        public IConfiguration Config { get; set; }

        public UnitOfDataPersistenceWork UnitOfWork { get; set; }

        public AADDTTest()
        {
            //_testHelper = new TestHelper();
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .Build();

            DbContext = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseSqlServer(Config.GetConnectionString("BridgeCareConnex"))
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

            UnitOfWork = new UnitOfDataPersistenceWork(Config, DbContext);
        }

        [Fact]
        public void ShouldMatchAADTTAttributeResult()
        {
            try
            {
                // Arrange

                // Act
                var maintainableAssets = UnitOfWork.MaintainableAssetRepo
                        .GetAllInNetworkWithAssignedDataAndLocations(NetworkId).ToList();

                // Create list of attribute ids we are allowed to update with assigned data.
                var networkAttributeIds = maintainableAssets.Where(_ => _.AssignedData != null && _.AssignedData.Any())
                    .SelectMany(_ => _.AssignedData.Select(__ => __.Attribute.Id).Distinct()).ToList();

                // convert meta data into attribute domain models
                var AADTTAttribute = AttributeFactory.Create(AADDTMetaDatum);
                var connection = AttributeConnectionBuilder.Build(AADTTAttribute);
                var AADTTAttributeData = AttributeDataBuilder.GetData(connection).ToList();

                // get the attribute ids for assigned data that can be deleted (attribute is present
                // in the data source and meta data file)
                var attributeIdsToBeUpdatedWithAssignedData = new List<Guid> { Guid.Parse("6a473634-ce64-4cae-acda-7306a2495454") };

                // loop over maintainable assets and remove assigned data that has an attribute id
                // in attributeIdsToBeUpdatedWithAssignedData then assign the new attribute data
                // that was created
                foreach (var maintainableAsset in maintainableAssets)
                {
                    maintainableAsset.AssignedData.Clear();
                    maintainableAsset.AssignAttributeData(AADTTAttributeData);
                }
                AggregateData(NetworkId, maintainableAssets);

                // Assert
                //Assert.IsType<OkObjectResult>(result.Result);
            }
            finally
            {
                // Cleanup
                //_testHelper.CleanUp();
                UnitOfWork.Dispose();
            }
        }
        private void AggregateData(Guid networkId, List<MaintainableAsset> maintainableAssets)
        {
            try
            {
                var aggregatedResults = new List<IAggregatedResult>();

                var totalAssests = (double)maintainableAssets.Count;
                // loop over the maintainable assets and aggregate the assigned data as numeric or
                // text based on assigned data attribute data type
                foreach (var maintainableAsset in maintainableAssets)
                {
                    // aggregate numeric data
                    if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "NUMBER"))
                    {
                        aggregatedResults.AddRange(maintainableAsset.AssignedData
                            .Where(_ => _.Attribute.DataType == "NUMBER")
                            .Select(_ => _.Attribute)
                            .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateNumericRule(_)))
                            .ToList());
                    }

                    //aggregate text data
                    if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "STRING"))
                    {
                        aggregatedResults.AddRange(maintainableAsset.AssignedData
                            .Where(_ => _.Attribute.DataType == "STRING")
                            .Select(_ => _.Attribute)
                            .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateTextRule(_)))
                            .ToList());
                    }
                }
                var entities = aggregatedResults.SelectMany(_ => _.ToEntity()).ToList();

                //var res = entities.GroupBy(l => l.Year)
                //    .Select(cl => new AggregatedResultEntity
                //    {
                //        NumericValue = cl.Sum(c => c.NumericValue),
                //        Year = cl.Key
                //    });

                var yearAndAverageValue = entities.GroupBy(p => p.Year,
                    p => p.NumericValue,
                    (key, g) => new { Year = key, Average = g.Average(_ => _.Value)}).ToList();
                // create aggregated data records in the data source
                //var createdRecordsCount = _testHelper.UnitOfWork.AggregatedResultRepo.CreateAggregatedResults(aggregatedResults);
            }
            catch (Exception e)
            {
                var test = 0;
            }
        }
    }
}
