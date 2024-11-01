using AppliedResearchAssociates.iAM.DTOs;
using System;

namespace BridgeCareCore.Services
{
    public interface IExcelRawDataLoadService
    {
        GetRawDataSpreadsheetColumnHeadersResultDTO GetSpreadsheetColumnHeaders(Guid dataSourceId);

    }
}
