using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelValueModels
    {
        public static ExcelMoneyValueModel Money(decimal amount)
            => new ExcelMoneyValueModel
            {
                Value = amount
            };

        public static ExcelIntegerValueModel Integer(int n)
            => new ExcelIntegerValueModel
            {
                Value = n
            };

        public static ExcelNothingModel Nothing
            => new ExcelNothingModel();

        public static ExcelStringValueModel String(string text)
            => new ExcelStringValueModel
            {
                Value = text,
            };

    }
}
