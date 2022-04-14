using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentRowModels
    {
        public static ExcelRowModel TreatmentNameRow(TreatmentDTO dto)
        {
            var firstCell = ExcelValueModels.RichString("Treatment Name", true);
            return ExcelRowModels.WithEntries(firstCell);
        }
    }
}
