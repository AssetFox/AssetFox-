using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentRowModels
    {
        public static ExcelRowModel TreatmentNameRow(TreatmentDTO dto)
        {
            var firstCell = ExcelValueModels.RichString(TreatmentExportStringConstants.TreatmentName, true);
            var nameCell = ExcelValueModels.String(dto.Name);
            return ExcelRowModels.WithEntries(firstCell, nameCell);
        }

        public static ExcelRowModel CriteriaRow(TreatmentDTO dto)
        {
            var constantCell = ExcelValueModels.RichString(TreatmentExportStringConstants.Criteria, true);
            var criteriaString = "value needed here";
            var nameCell = ExcelValueModels.String(criteriaString);
            return ExcelRowModels.WithEntries(constantCell, nameCell);
        }
    }
}
