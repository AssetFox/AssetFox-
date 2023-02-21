using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests
{
    public static class DataSourceTestSetup
    {
        public static SQLDataSourceDTO DtoForSqlDataSourceInDb(IUnitOfWork unitOfWork, string connectionString)
        {
            var dataSource = SqlDataSourceDtos.WithConnectionString(connectionString);
            unitOfWork.DataSourceRepo.UpsertDatasource(dataSource);
            return dataSource;
        }

        public static ExcelDataSourceDTO DtoForExcelDataSourceInDb(IUnitOfWork unitOfWork)
        {
            var dataSource = ExcelDataSourceDtos.WithColumnNames("Inspection_Date", "BRKEY");
            unitOfWork.DataSourceRepo.UpsertDatasource(dataSource);
            return dataSource;
        }       
    }
}
