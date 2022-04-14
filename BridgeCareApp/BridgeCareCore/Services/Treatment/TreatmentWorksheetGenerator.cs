using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;
using BridgeCareCore.Services.SummaryReport.Visitors;
using OfficeOpenXml;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentWorksheetGenerator
    {
        public static void Fill(ExcelWorkbook workbook, TreatmentLibraryDTO dto)
        {
            foreach (var treatment in dto.Treatments)
            {
                var worksheetModel = ExcelTreatmentModels.TreatmentWorksheet(treatment);
                ExcelWorksheetAdder.AddWorksheet(workbook, worksheetModel);
            }
        }
    }
}
