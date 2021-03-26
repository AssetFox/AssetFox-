using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy.Entities;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IBridgeDataForSummaryReport
    {
        WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData);
    }
}
