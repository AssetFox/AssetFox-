using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PamsData;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.ShortNameGlossary
{
    public static class ShortNameGlossaryTreatmentModels
    {

        public static RowBasedExcelRegionModel TreatmentsRows()
        {
            var returnValue = RowBasedExcelRegionModels.WithRows(
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
                returnValue.Rows.Add(row);
            }
            return returnValue;
        }
    }
}
