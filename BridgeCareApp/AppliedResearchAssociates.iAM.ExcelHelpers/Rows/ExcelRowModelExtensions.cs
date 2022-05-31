using System.Linq;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/Rows/ExcelRowModelExtensions.cs
namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/Rows/ExcelRowModelExtensions.cs
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
