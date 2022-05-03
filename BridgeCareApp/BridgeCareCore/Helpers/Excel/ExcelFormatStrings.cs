using System.Text;

namespace BridgeCareCore.Helpers.Excel
{
    public static class ExcelFormatStrings
    {
        public const string CurrencyWithoutCents = "_-$* #,##0_-;_-$* #,##0_-;_-$* \"-\"??_-;_-@_-";
        public const string Currency = "_-$* #,##0.00_-;_-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
        public const string NegativeCurrency = "_-$* #,##0_-;$* (#,##0)_-;_-$* \"-\"??_-;_-@_-";
        public static string Percentage(int digitsAfterDecimalPlace)
        {
            if (digitsAfterDecimalPlace == 0)
            {
                return "#0%";
            }
            var builder = new StringBuilder();
            builder.Append("#0.");
            for (int i = 0; i < digitsAfterDecimalPlace; i++)
            {
                builder.Append('0');
            }
            builder.Append('%');
            return builder.ToString();
        }
    }
}
