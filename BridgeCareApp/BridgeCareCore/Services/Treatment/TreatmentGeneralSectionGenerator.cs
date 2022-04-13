using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentGeneralSectionGenerator
    {
        public static ExcelRowModel TreatmentNameRow(TreatmentDTO dto)
        {
            var firstCell = ExcelValueModels.RichString("Treatment Name", true);
            return ExcelRowModels.WithEntries(firstCell);
        }
        public static List<IExcelWorksheetContentModel> GeneralSectionRows(
            TreatmentDTO dto
            ) {
            var r = new List<IExcelWorksheetContentModel>
            {
                TreatmentNameRow(dto),
            };
            return r;
        }
    }
}
