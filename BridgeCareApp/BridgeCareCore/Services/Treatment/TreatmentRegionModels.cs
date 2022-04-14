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
                TreatmentRowModels.CriteriaRow(dto),
                TreatmentRowModels.CategoryRow(dto),
                TreatmentRowModels.AssetTypeRow(dto),
                TreatmentRowModels.YearsBeforeAnyRow(dto),
                TreatmentRowModels.YearsBeforeSameRow(dto),
                TreatmentRowModels.TreatmentDescriptionRow(dto),
            };
            return RowBasedExcelRegionModels.WithRows(rows);
        }
    }
}
