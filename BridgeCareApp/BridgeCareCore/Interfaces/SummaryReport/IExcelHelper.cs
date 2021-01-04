﻿using System.Drawing;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IExcelHelper
    {
        void ApplyBorder(ExcelRange cells);
        void ApplyColor(ExcelRange cells, Color color);
        void ApplyStyle(ExcelRange cells);
        void MergeCells(ExcelWorksheet worksheet, int fromRow, int fromColumn, int toRow, int toColumn, bool makeTextBold = true);
        void SetCurrencyFormat(ExcelRange cells);
        void SetCustomFormat(ExcelRange cells, string type);
        void SetTextColor(ExcelRange cells, Color color);
    }
}