using System.Linq;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Rows
{
    public static class ExcelRowModelExtensions
    {
        public static void AddCells(this ExcelRowModel row, params IExcelModel[] cellContent)
        {
            foreach (var model in cellContent)
            {
                row.Values.Add(RelativeExcelRangeModels.OneByOne(model));
            }
        }

        public static void AddRepeated(this ExcelRowModel row, int count, IExcelModel cellContent)
        {
            var cells = Enumerable.Repeat(cellContent, count).ToArray();
            row.AddCells(cells);
        }
    }
}
