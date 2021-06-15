using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport
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
            var range = worksheet.Cells[rowIndex, columnIndex, rowIndex+width-1, columnIndex+height-1];
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
                writer.Write(model.Values[i].Content, worksheet, rowIndex, columnIndex, size.Width, size.Height);
                columnIndex += size.Width;
            }
        }

        public static void AddWorksheet(this ExcelWriter writer, ExcelWorkbook workbook, RowBasedExcelWorksheetModel worksheetModel)
        {
            var worksheet = workbook.Worksheets.Add(worksheetModel.TabName);
            for (int zeroBasedIndex = 0; zeroBasedIndex < worksheetModel.Rows.Count; zeroBasedIndex++)
            {
                writer.WriteRow(worksheetModel.Rows[zeroBasedIndex], worksheet, 1 + zeroBasedIndex);
            }
        }
    }
}
