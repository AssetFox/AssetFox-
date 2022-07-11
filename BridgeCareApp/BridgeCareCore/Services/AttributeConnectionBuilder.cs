using System;
using System.Data;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;

namespace BridgeCareCore.Services
{
    public static class AttributeConnectionBuilder
    {
        public static AttributeConnection Build(Attribute attribute, BaseDataSourceDTO dataSource, IUnitOfWork unitOfWork)
        {
            // wjwjwj run this thing.
            switch (attribute.ConnectionType)
            {
            case ConnectionType.MSSQL:
                return new SqlAttributeConnection(attribute, dataSource);

            case ConnectionType.EXCEL:
                var excelSpreadsheet = unitOfWork.ExcelWorksheetRepository.GetExcelRawDataByDataSourceId(dataSource.Id);
                if (excelSpreadsheet == null)
                {
                    var warningMessage = $@"Found DataSource {dataSource.Name}. The DataSource was of type ""EXCEL"". However, we did not find an ExcelRawData for that data source.";
                    throw new RowNotInTableException(warningMessage);
                }
                var worksheet = ExcelRawDataSpreadsheetSerializer.Deserialize(excelSpreadsheet.SerializedWorksheetContent).Worksheet;
                return new ExcelAttributeConnection(attribute, dataSource, worksheet);
            default:
                throw new InvalidOperationException($"Invalid Connection type \"{attribute.ConnectionType}\".");
            }
        }
    }
}
