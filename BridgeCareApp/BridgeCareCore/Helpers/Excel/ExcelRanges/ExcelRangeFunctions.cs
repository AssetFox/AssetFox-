using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace BridgeCareCore.Helpers.Excel
{
    public static class ExcelRangeFunctions
    {
        public static Func<ExcelRange, string> StartOffset(int columnDelta, int rowDelta, bool absoluteColumn = false, bool absoluteRow = false)
            => range =>
            {
                var start = range.Start;
                var offsetStart = ExcelCellAddressFunctions.Offset(start, columnDelta, rowDelta);
                var address = offsetStart.Address;
                var returnValue = ExcelAddressFunctions.ChangeAbsolute(address, absoluteColumn, absoluteRow);
                return returnValue;
            };

        /// <summary>
        /// Returns the left neighbor of the start of the range.
        /// For example, Left(B3:C4) = A3.
        /// </summary>
        public static Func<ExcelRange, string> Left =>
            range =>
                {
                    var start = range.Start;
                    var returnValue = ExcelCellAddressFunctions.Left(start).Address;
                    return returnValue;
                };
        /// <summary>Always returns the text, regardless of the range</summary>
        public static Func<ExcelRange, string> Constant(string text)
            => range => text;
        /// <summary>Always returns the empty string</summary> 
        public static Func<ExcelRange, string> Empty
            => Constant("");
        public static Func<ExcelRange, string> Concat(params Func<ExcelRange, string>[] functions)
            => range =>
                {
                    var builder = new StringBuilder();
                    foreach (var func in functions)
                    {
                        builder.Append(func(range));
                    }
                    var returnValue = builder.ToString();
                    return returnValue;
                };
        public static Func<ExcelRange, string> Plus(params Func<ExcelRange, string>[] summands)
        {
            if (summands.Any())
            {
                return range =>
                {
                    var builder = new StringBuilder();
                    builder.Append(summands[0](range));
                    for (int i = 1; i < summands.Length; i++)
                    {
                        builder.Append("+");
                        builder.Append(summands[i](range));
                    }
                    var returnValue = builder.ToString();
                    return returnValue;
                };
            }
            return Empty;
        }
        /// <summary>Any null arguments are repla,ced with the function that returns the empty string.
        /// If that's not what you want, see BuildExcelFunctionWithOptionalArguments.</summary>
        public static Func<ExcelRange, string> BuildExcelFunction(
            string functionName,
            IEnumerable<Func<ExcelRange, string>> arguments)
            => range =>
            {
                var builder = new StringBuilder();
                builder.Append($"{functionName}(");
                var argumentsArray = arguments.Select(x => x ?? Empty).ToArray();
                if (argumentsArray.Any())
                {
                    builder.Append(argumentsArray[0](range));
                    for (int i = 1; i < argumentsArray.Length; i++)
                    {
                        builder.Append($",");
                        var entry = argumentsArray[i](range);
                        builder.Append(entry);
                    }
                }
                builder.Append(")");
                var returnValue = builder.ToString();
                return returnValue;
            };

        public static Func<ExcelRange, string> BuildExcelFunctionWithOptionalArguments(
            string functionName,
            params Func<ExcelRange, string>[] arguments)
        {
            var argumentsWithoutTrailingNulls = arguments.ToList();
            while (argumentsWithoutTrailingNulls.Any() && argumentsWithoutTrailingNulls.Last() == null)
            {
                argumentsWithoutTrailingNulls.RemoveAt(argumentsWithoutTrailingNulls.Count - 1);
            }
            return BuildExcelFunction(functionName, argumentsWithoutTrailingNulls);
        }

        public static Func<ExcelRange, string> BuildExcelFunction(
            string functionName,
            params Func<ExcelRange, string>[] arguments)
            => BuildExcelFunction(functionName, arguments.AsEnumerable());

        public static Func<ExcelRange, string> SumIfs(IEnumerable<Func<ExcelRange, string>> arguments)
            => BuildExcelFunction("SUMIFS", arguments);

        public static Func<ExcelRange, string> Indirect(Func<ExcelRange, string> argument)
            => BuildExcelFunction("INDIRECT", argument);

        public static Func<ExcelRange, string> Address(
            Func<ExcelRange, string> row,
            Func<ExcelRange, string> column,
            Func<ExcelRange, string> optionalAbsolute = null,
            Func<ExcelRange, string> optionalStyle = null,
            Func<ExcelRange, string> optionalSheet = null
            )
            => BuildExcelFunctionWithOptionalArguments(
                "Address", row, column, optionalAbsolute, optionalStyle, optionalSheet);

        public static Func<ExcelRange, string> RangeSum(
            Func<ExcelRange, string> start,
            Func<ExcelRange, string> end)
            => range =>
            {
                var startValue = start(range);
                var endValue = end(range);
                return $"Sum({startValue}:{endValue})";
            };

        /// <summary>For example, if you pass in a range that starts at
        /// C8, then StartOffsetRangeSum(-3, 0, -1, 0) would return
        /// SUM(C5:C7).</summary>
        public static Func<ExcelRange, string> StartOffsetRangeSum(
            int dxStart, int dyStart, int dxEnd, int dyEnd)
        {
            var start = StartOffset(dxStart, dyStart);
            var end = StartOffset(dxEnd, dyEnd);
            return RangeSum(start, end);
        }
    }
}
