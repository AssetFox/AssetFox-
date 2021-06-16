using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelRangeFunctions
    {
        /// <summary>
        /// Returns the left neighbor of the start of the range.
        /// For example, Left(B3:C4) = A3.
        /// </summary>
        public static Func<ExcelRange, string> Left =>
            range =>
                {
                    var start = range.Start;
                    var r = ExcelCellAddressFunctions.Left(start).Address;
                    return r;
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
                    var r = builder.ToString();
                    return r;
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
                    var r = builder.ToString();
                    return r;
                };
            }
            return Empty;
        }
        /// <summary>Any null arguments are replaced with the function that returns the empty string.
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
                var r = builder.ToString();
                return r;
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
    }
}
