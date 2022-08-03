using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IExcelRawDataRepository
    {
        Guid AddExcelRawData(ExcelRawDataDTO dto);
        ExcelRawDataDTO GetExcelRawData(Guid excelPackageId);
        ExcelRawDataDTO GetExcelRawDataByDataSourceId(Guid dataSourceId);
    }
}
