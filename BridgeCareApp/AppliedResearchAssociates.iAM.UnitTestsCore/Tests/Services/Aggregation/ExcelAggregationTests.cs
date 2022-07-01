using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.SampleData;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services.Aggregation;
using OfficeOpenXml;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services.Aggregation
{
    public class ExcelAggregationTests
    {
        private static TestHelper _testHelper => TestHelper.Instance;

        private static string SampleAttributeDataPath()
        {
            var filename = SampleAttributeDataFilenames.Sample;
            var folder = Directory.GetCurrentDirectory();
            var returnValue = Path.Combine(folder, SampleAttributeDataFolderNames.SampleData, filename);
            return returnValue;
        }

        private static void EnsureDistrictAttributeExists()
        {
            var dto = AttributeDtos.District;
            AttributeTestSetup.EnsureAttributeExists(dto);
        }

        [Fact]
        public async Task Aggregate_ExcelDataSourceInDb_AttributesInDb_Aggregates()
        {
            var districtAttribute = AttributeDtos.District;
            AttributeTestSetup.EnsureAttributeExists(districtAttribute);
            var path = SampleAttributeDataPath();
            var stream = File.OpenRead(path);
            var excelPackage = new ExcelPackage(stream);
            var spreadsheetService = TestServices.CreateExcelSpreadsheetImportService(_testHelper.UnitOfWork);
            var dataSourceId = Guid.NewGuid();
            var dataSourceName = RandomStrings.WithPrefix("DataSourceName");
            var dataSourceDto = new ExcelDataSourceDTO
            {
                Id = dataSourceId,
                Name = dataSourceName,
            };
            _testHelper.UnitOfWork.DataSourceRepo.UpsertDatasource(dataSourceDto);
            var importResult = spreadsheetService.ImportRawData(dataSourceDto.Id, excelPackage.Workbook.Worksheets[0]);
            var explorer = new Explorer("Age");
            var network = explorer.AddNetwork();
            var networkName = RandomStrings.WithPrefix("Network");
            network.Name = networkName;

            _testHelper.UnitOfWork.NetworkRepo.CreateNetwork(network);
            var networkId = network.Id;
            var location = new SectionLocation(Guid.NewGuid(), assetName);
            var maintainableAssetId = Guid.NewGuid();
            var newAsset = new MaintainableAsset(maintainableAssetId, networkId, location, spatialWeightingValue); // wjwjwj this "[Deck_Area]" is wrong and will need to change

            var aggregationService = new AggregationService(_testHelper.UnitOfWork);

            var channel = Channel.CreateUnbounded<AggregationStatusMemo>();
            var aggregationState = new AggregationState();
            var attributes = new List<AttributeDTO> { districtAttribute };
            var aggregationResult = await aggregationService.AggregateNetworkData(channel.Writer, networkId, aggregationState, attributes);
            Assert.True(aggregationResult);
        }
    }
}
