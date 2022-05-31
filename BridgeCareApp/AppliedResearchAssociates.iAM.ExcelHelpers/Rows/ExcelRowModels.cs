﻿using System.Collections.Generic;
using System.Linq;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/Rows/ExcelRowModels.cs
namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/Rows/ExcelRowModels.cs
{
    public static class ExcelRowModels
    {
        public static ExcelRowModel WithEntries(params IExcelModel[] entries)
            => WithEntries(entries.ToList());

        /// <summary>The everyCell models will be applied to every cell in the row.
        /// They might typically be used for styles such as bold, the border, or alignment.</summary>
        public static ExcelRowModel WithEntries<T>(List<T> entries, params IExcelModel[] everyCell)
            where T: IExcelModel
            => new ExcelRowModel
            {
                Values = entries.Select(x => RelativeExcelRangeModels.OneByOne(x)).ToList(),
                EveryCell = StackedExcelModels.Stacked(everyCell),
            };

        public static ExcelRowModel Empty
            => WithEntries();


        public static ExcelRowModel WithCells(params RelativeExcelRangeModel[] cells)
            => new ExcelRowModel
            {
                Values = cells.ToList(),
            };

        public static ExcelRowModel WithCells(List<RelativeExcelRangeModel> cells)
            => new ExcelRowModel
            {
                Values = cells,
            };

        public static ExcelRowModel IndentedHeader(int indentColumns, string headerText, int headerWidth, int headerHeight)
        {
            var values = new List<RelativeExcelRangeModel>();
            for (int i=0; i<indentColumns; i++)
            {
                values.Add(RelativeExcelRangeModels.Empty());
            }
            values.Add(RelativeExcelRangeModels.BoldText(headerText, headerWidth, headerHeight));
            return WithCells(values);
        }
    }
}
