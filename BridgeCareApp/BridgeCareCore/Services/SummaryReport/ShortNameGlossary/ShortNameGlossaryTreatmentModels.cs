using BridgeCareCore.Services.SummaryReport.BridgeData;
using BridgeCareCore.Helpers.Excel;

namespace BridgeCareCore.Services.SummaryReport.ShortNameGlossary
{
    public static class ShortNameGlossaryTreatmentModels
    {

        public static RowBasedExcelRegionModel TreatmentsRows()
        {
            var r = RowBasedExcelRegionModels.WithRows(
                    ExcelRowModels.WithEntries(
                        StackedExcelModels.LeftHeaderWrap("Bridge Care Work Type"),
                        StackedExcelModels.LeftHeaderWrap("Short Bridge Care Work type"))
                );
            var abbreviatedTreatmentNames = ShortNamesForTreatments.GetShortNamesForTreatments();
            foreach (var treatment in abbreviatedTreatmentNames)
            {
                var row = ExcelRowModels.WithEntries(
                    ExcelValueModels.String(treatment.Key),
                    ExcelValueModels.String(treatment.Value));
                row.EveryCell = ExcelStyleModels.ThinBorder;
                r.Rows.Add(row);
            }
            return r;
        }
    }
}
