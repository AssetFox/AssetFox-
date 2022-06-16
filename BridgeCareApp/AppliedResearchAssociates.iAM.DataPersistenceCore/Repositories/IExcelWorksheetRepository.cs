using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IExcelWorksheetRepository
    {
        Guid AddExcelWorksheet(ExcelSpreadsheetDTO dto);
    }
}
