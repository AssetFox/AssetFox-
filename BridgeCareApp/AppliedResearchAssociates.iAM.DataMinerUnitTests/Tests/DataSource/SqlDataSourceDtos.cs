using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests
{
    public static class SqlDataSourceDtos
    {
        public static SQLDataSourceDTO WithConnectionString(string connectionString)
        {
            var randomName = RandomStrings.WithPrefix("TestDataSource");
            var sqlDataSource = new SQLDataSourceDTO
            {
                ConnectionString = connectionString,
                Id = Guid.NewGuid(),
                Name = randomName,
            };
            return sqlDataSource;
        }
    }
}
