using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
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

        public static void AddCells(this ExcelRowModel row, IEnumerable<IExcelModel> cellContent)
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
