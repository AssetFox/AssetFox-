using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class DataSourceDtoPropertyTests
    {
      /*The AllDataSource type should have all properties of
        the types SqlDataSourceDTO and ExcelDataSourceDTO. This
        is necessary for the front end. These tests check that it
        remains true.*/
        [Fact]
        public void AllDataSourceHasAllPropertiesOfSqlDataSourceDto()
        {
            var bigType = typeof(AllDataSource);
            var littleType = typeof(SQLDataSourceDTO);
            var bigTypeProperties = bigType.GetProperties();
            var littleTypeProperties = littleType.GetProperties();
            foreach (var littleTypeField in littleTypeProperties)
            {
                var littleName = littleTypeField.Name;
                var bigTypeHasFieldWithSameName = bigTypeProperties.Any(f => f.Name == littleName);
                Assert.True(bigTypeHasFieldWithSameName);
            }
        }


        [Fact]
        public void AllDataSourceHasAllPropertiesOfExcelDataSourceDto()
        {
            var bigType = typeof(AllDataSource);
            var littleType = typeof(ExcelDataSourceDTO);
            var bigTypeProperties = bigType.GetProperties();
            var littleTypeProperties = littleType.GetProperties();
            foreach (var littleTypeField in littleTypeProperties)
            {
                var littleName = littleTypeField.Name;
                var bigTypeHasFieldWithSameName = bigTypeProperties.Any(f => f.Name == littleName);
                Assert.True(bigTypeHasFieldWithSameName);
            }
        }
    }
}
