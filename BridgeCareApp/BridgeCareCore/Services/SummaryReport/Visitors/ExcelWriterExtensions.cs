using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.Visitors
{
    public static class ExcelWriterExtensions
    {
        public static void Write(
            this ExcelWriter writer,
            IExcelModel model,
            ExcelRange range)
        {
            model.Accept(writer, range);
        }

        /// <summary>Indexes are one-based.</summary>
        public static void Write(
            this ExcelWriter writer,
            IExcelModel model,
            ExcelWorksheet worksheet,
            int rowIndex,
            int columnIndex,
            int width,
            int height)
        {
            var range = worksheet.Cells[rowIndex, columnIndex, rowIndex+height-1, columnIndex+width-1];
            if (width > 1 || height > 1)
            {
                range.Merge = true;
            }
            writer.Write(model, range);
        }

        /// <summary>rowIndex is one-based.</summary>
        public static void WriteRow(
            this ExcelWriter writer,
            ExcelRowModel model,
            ExcelWorksheet worksheet,
            int rowIndex
            )
        {
            var columnIndex = 1;
            for (int i = 0; i < model.Values.Count; i++)
            {
                var size = model.Values[i].Size;
                var range = worksheet.Cells[rowIndex, columnIndex, rowIndex + size.Height - 1, columnIndex + size.Width - 1];
                if (size.Width > 1 || size.Height > 1)
                {
                    range.Merge = true;
                }
                foreach (var every in model.EveryCell.Content)
                {
                    writer.Write(every, range);
                }
                writer.Write(model.Values[i].Content, range);
                columnIndex += size.Width;
            }
        }

        public static ExcelWorksheet WriteWorksheet(this ExcelWriter writer, ExcelWorksheet worksheet, RowBasedExcelWorksheetContentModel worksheetModel)
        {
            for (int i = 0; i < worksheetModel.Rows.Count; i++)
            {
                writer.WriteRow(worksheetModel.Rows[i], worksheet, 1 + i);
            }
            return worksheet;
        }
    }
}
