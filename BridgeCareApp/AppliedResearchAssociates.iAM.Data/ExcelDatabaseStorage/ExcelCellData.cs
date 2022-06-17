using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.CellData;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    public static class ExcelCellData
    {
        private static EmptyExcelCellDatum _Empty { get; } = new EmptyExcelCellDatum();
        public static DateTimeExcelCellDatum DateTime(DateTime dateTime)
            => new DateTimeExcelCellDatum
            {
                Value = dateTime,
            };

        public static StringExcelCellDatum String(string str)
            => new StringExcelCellDatum
            {
                Value = str,
            };

        public static DoubleExcelCellDatum Double(double value)
            => new DoubleExcelCellDatum
            {
                Value = value,
            };

        public static EmptyExcelCellDatum Empty
            => _Empty;

        public static IExcelCellDatum ForObject(object content)
        {
            IExcelCellDatum newCell = null;
            if (content == null || content is string str && string.IsNullOrWhiteSpace(str))
            {
                newCell = Empty;
            }
            else if (content is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                case JsonValueKind.Number:
                    if (jsonElement.TryGetDouble(out double d))
                    {
                        newCell = Double(d);
                    }
                    break;
                case JsonValueKind.String:
                    if (jsonElement.TryGetDateTime(out DateTime dt))
                    {
                        newCell = DateTime(dt);
                    }
                    else
                    {
                        var s = jsonElement.GetString();
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            newCell = Empty;
                        }
                        else
                        {
                            newCell = String(s);
                        }
                    }
                    break;
                }
            }
            else if (content is double doubleValue)
            {
                newCell = Double(doubleValue);
            }
            else if (content is int intValue)
            {
                newCell = Double(intValue);
            }
            else if (content is DateTime dateTimeValue)
            {
                newCell = DateTime(dateTimeValue);
            }
            else if (content is float floatValue)
            {
                newCell = Double(floatValue);
            }
            else
            {
                newCell = String(content?.ToString() ?? "");
            }

                return newCell;
            }
        }
    }
