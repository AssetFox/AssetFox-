using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class ExcelDatabaseWorksheetMapper
    {
        public static ExcelWorksheetEntity ToEntity(this ExcelDatabaseWorksheet worksheet)
        {
            var serializedContent = ExcelDatabaseWorksheetSerializer.Serialize(worksheet);
            var returnValue = new ExcelWorksheetEntity
            {
                Id = Guid.Empty,
                SerializedWorksheetContent = serializedContent,
            };
            return returnValue;
        }
    }
}
