using System.Linq;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Models/BAMSSummaryReport/Rows/ExcelRowModelExtensions.cs
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Rows
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/Rows/ExcelRowModelExtensions.cs
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
