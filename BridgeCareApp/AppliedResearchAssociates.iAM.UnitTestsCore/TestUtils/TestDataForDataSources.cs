using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestDataForDataSources
    {
        public static Guid SqlDatasourceId => new Guid("72b3cca4-57f1-4e0d-ad13-37c2664f1299");
        public static Guid ExcelDatasourceId => new Guid("147cb3e1-e9fc-4fd6-a265-105d546d9ddb");

        public const string ExcelDatasourceDateColumn = "Date";
        public const string ExcelDatasourceLocationColumn = "Location";
        public const string SqlServerDatasourceName = "SQL Server Data Source";
        public const string ExcelDatasourceName = "Some Excel File";
        public const string ExcelDatasourceDetails = "{\"LocationColumn\":\"Location\",\"DateColumn\":\"Date\"}";

        private static SQLDataSourceDTO SqlDataSourceDto()
        {
            return new SQLDataSourceDTO
            {
                Id = SqlDatasourceId,
                Name = SqlServerDatasourceName,
                Type = DataSourceTypeStrings.SQL.ToString(),
            };
        }

        private static ExcelDataSourceDTO ExcelDataSourceDto()
        {
            return new ExcelDataSourceDTO
            {
                Id = ExcelDatasourceId,
                Name = ExcelDatasourceName,
                Type = DataSourceTypeStrings.Excel.ToString(),
                DateColumn = ExcelDatasourceDateColumn,
                LocationColumn = ExcelDatasourceLocationColumn,
            };
        }

        public static List<BaseDataSourceDTO> SourceDTOs()
        {
            var dataSources = new List<BaseDataSourceDTO>
            {
                SqlDataSourceDto(),
                ExcelDataSourceDto(),
            };
            return dataSources;
        }

    }

    public class BadExcelDataSourceDTO : ExcelDataSourceDTO
    {
        public override bool Validate() => false;
    }
}
