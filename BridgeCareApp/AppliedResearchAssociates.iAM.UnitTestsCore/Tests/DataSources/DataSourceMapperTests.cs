using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DataSources
{
    public class DataSourceMapperTests
    {
        [Fact]
        public void DataSourceEntity_MapToDto_Expected()
        {
            var dataSourceId = Guid.NewGuid();
            var entity = new DataSourceEntity
            {
                Id = dataSourceId,
                Type = "SQL",
                Name = "SQL Server Data Source",
                CreatedDate = new DateTime(2022, 10, 31),
                Details = Aes256GcmTests.CypherText,
            };
            var dto = DataSourceMapper.ToDTO(entity, Aes256GcmTests.Key);
            var expected = new SQLDataSourceDTO
            {
                ConnectionString = null,
                Name = "SQL Server Data Source",
                Id = dataSourceId,
            };
            ObjectAssertions.Equivalent(expected, dto);
        }

        [Fact]
        public void SimpleRepoEntityZero_MapToDto_Expected()
        {
            var entity = TestEntitiesForDataSources.SimpleRepo()[0];
            var dto = DataSourceMapper.ToDTO(entity, Aes256GcmTests.Key);
            var expected = new SQLDataSourceDTO
            {
                ConnectionString = null,
                Name = entity.Name,
                Id = entity.Id,
            };
            ObjectAssertions.Equivalent(expected, dto);
        }
    }
}
