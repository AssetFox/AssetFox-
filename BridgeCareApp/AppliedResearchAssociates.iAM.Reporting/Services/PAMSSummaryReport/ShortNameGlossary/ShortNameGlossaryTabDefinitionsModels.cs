using System.Collections.Generic;

using AppliedResearchAssociates.iAM.ExcelHelpers;
namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.ShortNameGlossary
{
    public static class ShortNameGlossaryTabDefinitionsModels
    {
        public static RowBasedExcelRegionModel GlossaryColumn()
        {
            var content = TabDefinitionsContent();
            return RowBasedExcelRegionModels.Column(content);
        }

        private static List<IExcelModel> TabDefinitionsContent()
            => new List<IExcelModel>
            {
                TabDefinitions,
                BoldThenNot("Parameters:", "Summary of simulation data inputs."),
                BoldThenNot("Bridge Data:", "Complete recommended treatments for all structures throughout the analysis period"),
                BoldThenNot("Unfunded Treatment \u2014 Final List", "First year of eligible but unfunded treatments by BRKEY."),
                BoldThenNot("Unfunded Treatments \u2014 Time:", "List of unfunded treatments through time."),
                BoldThenNot("Bridge Work Summary:", "Summary of projected costs for treatments throughout simulation period for all bridges. Summary of projected condition changes throughout analysis period."),
                BoldThenNot("Bridge Work Summary by Budget:", "Summary of projected costs for treatments throughout simulation period for all bridges by specific budget."),
                BoldThenNot("District Totals:", "Projected spending per district each year throughout analysis period."),
                BoldThenNot("NHS Count:", "Graph of projected bridge conditions for structures on the NHS by bridge count."),
                BoldThenNot("NHS DA:", "Graph of projected bridge conditions for structures on the NHS by deck area."),
                BoldThenNot("Non-NHS Count:", "Graph of projected bridge conditions for structures not on the NHS by bridge count."),
                BoldThenNot("Non-NHS DA:", "Graph of projected bridge conditions for structures not on the NHS by deck area."),
                BoldThenNot("Combined Count:", "Graph of projected bridge conditions for all structures by bridge count."),
                BoldThenNot("Combined DA:", "Graph of projected bridge conditions for all structures by deck area."),
                BoldThenNot("Poor Count:", "Graph showing projected number of structures to be in poor condition throughout analysis period."),
                BoldThenNot("Poor DA:", "Graph showing projected deck area of structures to be in poor condition throughout analysis period."),
                BoldThenNot("Poor DA by BPN:", "Graph showing projected deck area of structures to be in poor condition, by BPN, throughout analysis period."),
                BoldThenNot("Posted BPN Count:", "Graph showing projected number of structures to be posted based on off the GCR Super and not by load capacity calculations. Since it is not possible to perform load calculations ant this level, this can be considered a proxy and should be used only for only long-term evaluation of in change in total quantity."),
                BoldThenNot("Posted BPN DA:", "Graph showing projected deck area of structures to be posted based on the GCR Super and not by load capacity calculations. Since it is not possible to perform load calculations at this level, this can be considered a proxy and should be used only for long-term evaluation of change in total quantity."),
            };


        private static IExcelModel BoldThenNot(string bold, string notBold)
            => StackedExcelModels.Stacked(
                ExcelValueModels.RichString(bold, true),
                ExcelValueModels.RichString($" {notBold}")
            );

        private static IExcelModel TabDefinitions
            => ExcelValueModels.RichString("Tab Definitions", true);

    }
}
