using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class ExcelDatabaseWorksheetMapper
    {
        public static ExcelRawDataEntity ToEntity(this ExcelRawDataDTO dto)
        {
            var returnValue = new ExcelRawDataEntity
            {
                Id = dto.Id,
                DataSourceId = dto.DataSourceId,
                SerializedContent = dto.SerializedWorksheetContent,
            };
            return returnValue;
        }

        public static ExcelRawDataDTO ToDTO(this ExcelRawDataEntity entity)
        {
            var returnValue = new ExcelRawDataDTO
            {
                Id = entity.Id,
                SerializedWorksheetContent = entity.SerializedContent,
            };
            return returnValue;
        }

        internal static ExcelRawDataDTO ToDTONullPropagating(ExcelRawDataEntity entity)
        {
            if (entity == null)
            {
                return null;
            }
            return ToDTO(entity);
        }
    }
}
