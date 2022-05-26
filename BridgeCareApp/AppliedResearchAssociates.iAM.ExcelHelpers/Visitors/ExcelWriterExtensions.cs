using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.ExcelHelpers.Visitors
{
    public static class ExcelWriterExtensions
    {
        public static void Write(
            this ExcelWriter writer,
            IExcelModel model,
            ExcelRange range) => model.Accept(writer, range);

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
            int rowIndex,
            int startColumnIndex
            )
        {
            var columnIndex = startColumnIndex;
            for (int i = 0; i < model.Values.Count; i++)
            {
                var size = model.Values[i].Size;
                var range = worksheet.Cells[rowIndex, columnIndex, rowIndex + size.Height - 1, columnIndex + size.Width - 1];
                if (size.Width > 1 || size.Height > 1)
                {
                    range.Merge = true;
                }
                writer.Write(model.EveryCell, range);
                writer.Write(model.Values[i].Content, range);
                columnIndex += size.Width;
            }
        }

        public static ExcelWorksheet WriteRegion(this ExcelWriter writer, ExcelWorksheet worksheet, RowBasedExcelRegionModel worksheetModel, int startRow, int startColumn)
        {
            for (int i = 0; i < worksheetModel.Rows.Count; i++)
            {
                writer.WriteRow(worksheetModel.Rows[i], worksheet, startRow + i, startColumn);
            }
            return worksheet;
        }

        public static ExcelWorksheet WriteRegion(this ExcelWriter writer, ExcelWorksheet worksheet, RowBasedExcelRegionModel region, ExcelCellAddress startAtAddress)
        {
            var row = startAtAddress.Row;
            var column = startAtAddress.Column;
            writer.WriteRegion(worksheet, region, row, column);
            return worksheet;
        }
    }
}
