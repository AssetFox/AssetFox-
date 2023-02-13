using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public static class TestEntitiesForDataSources
    {
        public static List<DataSourceEntity> SimpleRepo()
        {
            var repo = new List<DataSourceEntity>();
            repo.Add(new DataSourceEntity
            {
                Id = TestDataForDataSources.SqlDatasourceId,
                Name = TestDataForDataSources.SqlServerDatasourceName,
                Secure = true,
                Type = DataSourceTypeStrings.SQL.ToString(),
                Details = "Fj6y4slHYqm2PAw/MyIepQLXxHU87hg87svgP5MaqwxQqLtZBmDi1f5rHyj0s35LNWCELke1cmb2p9iV/GyQxjxsNzbfHKyZdI5m5HlSiMWEihoS1aoFnKoiMUDrb8mD6+B+lXFQ5e/G3SqUvgRTLfQTjBoQ1CvEnklT7SnqWvJBB6sVXdXhcYiBQWjkCzXFHDVMPueOTFn6eiZd/8QE+Uwk6smc7hIihQb+OxcpYiZ7Qoy/NtXowmgI/IkJOjaJklo28B3zAg=="
            });
            repo.Add(new DataSourceEntity
            {
                Id = TestDataForDataSources.ExcelDatasourceId,
                Name = TestDataForDataSources.ExcelDatasourceName,
                Secure = false,
                Type = DataSourceTypeStrings.Excel.ToString(),
                Details = TestDataForDataSources.ExcelDatasourceDetails,
            });
            repo.Add(new DataSourceEntity
            {
                Id = new Guid("0f87bc68-41aa-41cb-a8ce-e0352ee23d40"),
                Name = "Unknown Data Source Type",
                Secure = false,
                Type = "Other",
                Details = "Cannot do anything"
            });
            return repo;
        }

        public static List<AttributeEntity> SimpleAttributeRepo()
        {
            var repo = new List<AttributeEntity>();
            repo.Add(new AttributeEntity
            {
                Id = new Guid("2e3ae9ac-c14c-46ab-8e4b-f93312bc8637"),
                Name = "Location",
                DataType = "STRING",
                AggregationRuleType = "PREDOMINANT",
                DataSource = SimpleRepo().First(_ => _.Type == "SQL")
            });
            repo.Add(new AttributeEntity
            {
                Id = new Guid("104bd958-8e0a-403c-b065-07d5e91eb27b"),
                Name = "AssetSize",
                DataType = "NUMBER",
                AggregationRuleType = "AVERAGE",
                DataSource = SimpleRepo().First(_ => _.Type == "SQL")
            });
            repo.Add(new AttributeEntity
            {
                Id = new Guid("9151b85a-a980-4301-a102-a6e0c301c193"),
                Name = "Bad",
                DataType = "STRING",
                AggregationRuleType = "PREDOMINANT",
                DataSource = null
            });
            return repo;
        }
    }
}
