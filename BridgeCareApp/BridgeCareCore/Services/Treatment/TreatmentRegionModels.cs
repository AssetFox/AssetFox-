using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentRegionModels
    {
        internal static RowBasedExcelRegionModel GeneralRegion(TreatmentDTO dto)
        {
            var rows = new List<ExcelRowModel>
            {
                TreatmentRowModels.TreatmentNameRow(dto),
            };
            return RowBasedExcelRegionModels.WithRows(rows);
        }
    }
}
