using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataUnitTests;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DataSources
{
    public static class DataSourceDtos
    {
        public static SQLDataSourceDTO TestConfigurationSql(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var config = TestConfiguration.Get();
            var name = RandomStrings.WithPrefix("dataSource");
            var connectionString = config.GetConnectionString("BridgeCareConnex");
            var dto = new SQLDataSourceDTO
            {
                ConnectionString = connectionString,
                Id = resolveId,
                Name = name,
            };
            return dto;
        }
    }
}
