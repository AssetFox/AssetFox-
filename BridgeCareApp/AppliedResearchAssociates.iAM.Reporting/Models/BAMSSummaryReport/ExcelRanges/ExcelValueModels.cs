namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges
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

        public static ExcelDecimalValueModel Decimal(decimal x)
            => new ExcelDecimalValueModel
            {
                Value = x
            };

        public static ExcelNothingModel Nothing
            => new ExcelNothingModel();

        public static ExcelStringValueModel String(string text)
            => new ExcelStringValueModel
            {
                Value = text,
            };

        public static ExcelRichTextModel RichString(string text, bool bold = false)
            => new ExcelRichTextModel { Text = text, Bold = bold };
    }
}
