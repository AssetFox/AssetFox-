using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public static class ExcelFormulas
    {
        public static string Sum(params string[] addresses)
        {
            var builder = new StringBuilder();
            builder.Append("Sum(");
            var first = true;
            foreach (var address in addresses)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(",");
                }
                builder.Append(address);
            }
            builder.Append(")");
            var returnValue = builder.ToString();
            return returnValue;
        }
        public static string Sum(params ExcelRange[] ranges)
        {
            var addresses = ranges.Select(r => r.Address).ToArray();
            return Sum(addresses);
        }

        public static string RangeSum(string startAddress, string endAddress)
            => $"Sum({startAddress}:{endAddress})";

        public static string CellAddress(int rowIndex, int columnIndex)
        {
            var columnName = ExcelCellAddress.GetColumnLetter(columnIndex);
            return $"{columnName}{rowIndex}";
        }

        public static string Sum(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            var fromCell = CellAddress(fromRow, fromColumn);
            var toCell = CellAddress(toRow, toColumn);
            return $"Sum({fromCell}:{toCell})";
        }

        public static string Percentage(int rowNumenator, int colNumenator, int rowDenominator, int colDenominator)
        {
            var numenatorCell = CellAddress(rowNumenator, colNumenator);
            var denominatorCell = CellAddress(rowDenominator, colDenominator);

            return $"({numenatorCell}/{denominatorCell})";
        }
    }
}
