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
        public static ExcelWorksheetEntity ToEntity(this ExcelSpreadsheetDTO dto)
        {
            var returnValue = new ExcelWorksheetEntity
            {
                Id = dto.Id,
                SerializedWorksheetContent = dto.SerializedWorksheetContent,
            };
            return returnValue;
        }

        public static ExcelSpreadsheetDTO ToDTO(this ExcelWorksheetEntity entity)
        {
            var returnValue = new ExcelSpreadsheetDTO
            {
                Id = entity.Id,
                SerializedWorksheetContent = entity.SerializedWorksheetContent,
            };
            return returnValue;
        }

        public static ExcelSpreadsheetDTO ToDTO (this ExcelDatabaseWorksheet worksheet, Guid id)
        {
            var serializedContent = ExcelDatabaseWorksheetSerializer.Serialize(worksheet);
            var returnValue = new ExcelSpreadsheetDTO
            {
                Id = id,
                SerializedWorksheetContent = serializedContent,
            };
            return returnValue;
        }

        internal static ExcelSpreadsheetDTO ToDTONullPropagating(ExcelWorksheetEntity entity)
        {
            if (entity == null)
            {
                return null;
            }
            return ToDTO(entity);
        }
    }
}
