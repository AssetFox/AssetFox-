using System;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
{
    public class ExcelRawDataLoadService : IExcelRawDataLoadService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExcelRawDataLoadService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public GetRawDataSpreadsheetColumnHeadersResultDTO GetSpreadsheetColumnHeaders(Guid dataSourceId)
        {
            var dataSource = _unitOfWork.DataSourceRepo.GetDataSource(dataSourceId);
            string warningMessage = null;
            if (dataSource == null)
            {
                warningMessage = $"No dataSource found with id {dataSourceId}";
            }
            else if (dataSource.Type.ToUpperInvariant() != "EXCEL")
            {
                warningMessage = @$"DataSource found. Its type was {dataSource.Type}. Expected the type to be ""EXCEL""";
            }

            if (warningMessage != null)
            {
                return new GetRawDataSpreadsheetColumnHeadersResultDTO
                {
                    WarningMessage = warningMessage,
                };
            }

            var excelSpreadsheet = _unitOfWork.ExcelWorksheetRepository.GetExcelRawDataByDataSourceId(dataSourceId);
            if (excelSpreadsheet == null)
            {
                warningMessage = $@"Found a DataSource with id {dataSourceId}. The DataSource was of type ""EXCEL"". However, we did not find an ExcelRawData with DataSourceId {dataSourceId}. This is unexpected.";
                return new GetRawDataSpreadsheetColumnHeadersResultDTO
                {
                    WarningMessage = warningMessage,
                };
            }

            var worksheet = ExcelRawDataSpreadsheetSerializer.Deserialize(excelSpreadsheet.SerializedWorksheetContent);
            var columnHeaders = worksheet.Worksheet.Columns.Select(c => c.Entries[0].ObjectValue().ToString()).ToList();
            return new GetRawDataSpreadsheetColumnHeadersResultDTO
            {
                ColumnHeaders = columnHeaders,
            };
        }
    }
}
