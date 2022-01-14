using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attribute_Tests
{
    //TODO: This class need to be rewritten. This involves getting data from PennDot_Report_A and IAMV2 is does not have this table.
    //Also cleaned up some code which was using repos from DataPersistance and DataMinder projects. I am not sure if this belongs to UnitTestsCore.
    //Need to check with Team on more information/details on this...
    public class AADDTTest
    {
        private AttributeMetaDatum AADDTMetaDatum;
        private static readonly Guid NetworkId = Guid.Parse("7f4ea3ba-6082-4e1e-91a4-b80578aeb0ed"); //7f4ea3ba-6082-4e1e-91a4-b80578aeb0ed
        //D7B54881-DD44-4F93-8250-3D4A630A4D3B                
        private readonly TestHelper _testHelper;
        public readonly IAMContext DbContext;

        public IConfiguration Config { get; set; }

        public UnitOfDataPersistenceWork UnitOfWork { get; set; }

        public AADDTTest()
        {
            _testHelper = new TestHelper();                        
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();            
            _testHelper.CreateSimulation();
            
            _testHelper.SetupDefaultHttpContext();
            UnitOfWork = _testHelper.UnitOfWork;
            AADDTMetaDatum= new AttributeMetaDatum()
            {
                Name = "ADTTOTAL",

                Id = Guid.Parse("6a473634-ce64-4cae-acda-7306a2495454"),

                DefaultValue = "1000",

                Minimum = 0.0,

                Maximum = 0.0,

                ConnectionString = _testHelper.Config.GetConnectionString("BridgeCareConnex"),

                DataSource = "MSSQL",

                Type = "NUMBER",

                Location = "section",

                Command = "SELECT CAST(BRKEY AS int) AS ID_, NULL AS ROUTES, NULL AS BEGIN_STATION, NULL AS END_STATION, NULL AS DIRECTION, CAST(BRKEY AS VARCHAR(MAX)) AS FACILITY, BRIDGE_ID AS SECTION, NULL AS SAMPLE_, CAST(INSPDATE AS DATETIME) AS DATE_, CAST(REPLACE(ADTTOTAL, ',', '') AS float) AS DATA_ FROM dbo.PennDot_Report_A WHERE (ISNUMERIC(ADTTOTAL) = 1)",

                AggregationRule = "AVERAGE",

                IsCalculated = false,

                IsAscending = true
            };
        }

        [Fact]
        public void ShouldMatchAADTTAttributeResult()
        {
            try
            {
                // Arrange                
                var AADTTAttribute = AttributeFactory.Create(AADDTMetaDatum);
                var connection = AttributeConnectionBuilder.Build(AADTTAttribute);
                var AADTTAttributeData = AttributeDataBuilder.GetData(connection).ToList();

                // Assert
                Assert.NotNull(AADTTAttributeData);
            }
            finally
            {
                // Cleanup
                //_testHelper.CleanUp();
                UnitOfWork.Dispose();
            }
        }

        //private void AggregateData(Guid networkId, List<MaintainableAsset> maintainableAssets)
        //{
        //    try
        //    {
        //        var aggregatedResults = new List<IAggregatedResult>();

        //        var totalAssests = (double)maintainableAssets.Count;
        //        // loop over the maintainable assets and aggregate the assigned data as numeric or
        //        // text based on assigned data attribute data type
        //        foreach (var maintainableAsset in maintainableAssets)
        //        {
        //            // aggregate numeric data
        //            if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "NUMBER"))
        //            {
        //                aggregatedResults.AddRange(maintainableAsset.AssignedData
        //                    .Where(_ => _.Attribute.DataType == "NUMBER")
        //                    .Select(_ => _.Attribute)
        //                    .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateNumericRule(_)))
        //                    .ToList());
        //            }

        //            //aggregate text data
        //            if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "STRING"))
        //            {
        //                aggregatedResults.AddRange(maintainableAsset.AssignedData
        //                    .Where(_ => _.Attribute.DataType == "STRING")
        //                    .Select(_ => _.Attribute)
        //                    .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateTextRule(_)))
        //                    .ToList());
        //            }
        //        }
        //        var entities = aggregatedResults.SelectMany(_ => _.ToEntity()).ToList();

        //        //var res = entities.GroupBy(l => l.Year)
        //        //    .Select(cl => new AggregatedResultEntity
        //        //    {
        //        //        NumericValue = cl.Sum(c => c.NumericValue),
        //        //        Year = cl.Key
        //        //    });

        //        var yearAndAverageValue = entities.GroupBy(p => p.Year,
        //            p => p.NumericValue,
        //            (key, g) => new { Year = key, Average = g.Average(_ => _.Value) }).ToList();
        //        // create aggregated data records in the data source
        //        //var createdRecordsCount = _testHelper.UnitOfWork.AggregatedResultRepo.CreateAggregatedResults(aggregatedResults);
        //    }
        //    catch (Exception e)
        //    {
        //        var test = 0;
        //    }
        //}
    }
}
