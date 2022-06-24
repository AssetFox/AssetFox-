using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PamsData;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.ShortNameGlossary
{
    public static class ShortNameGlossaryWorkTypeModels
    {

        public static RowBasedExcelRegionModel WorkTypesRows()
        {
            var returnValue = RowBasedExcelRegionModels.WithRows(
                    ExcelRowModels.WithEntries(
                        StackedExcelModels.LeftHeaderWrap("PAMS Work Type"),
                        StackedExcelModels.LeftHeaderWrap("Short PAMS Work type"))
                );
            var abbreviatedWorkTypeNames = ShortNamesForWorkTypes.GetShortNamesForWorkTypes();
            foreach (var workType in abbreviatedWorkTypeNames)
            {
                var row = ExcelRowModels.WithEntries(
                    ExcelValueModels.String(workType.Key),
                    ExcelValueModels.String(workType.Value));
                row.EveryCell = ExcelStyleModels.ThinBorder;
                returnValue.Rows.Add(row);
            }
            return returnValue;
        }
    }
}
