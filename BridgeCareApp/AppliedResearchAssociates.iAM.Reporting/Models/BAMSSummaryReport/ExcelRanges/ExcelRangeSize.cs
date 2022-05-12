namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges
{
    public class ExcelRangeSize
    {
        public ExcelRangeSize(int width = 1, int height = 1)
        {
            Width = width;
            Height = height;
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public override string ToString() => $"{nameof(ExcelRangeSize)} {Width}x{Height}";
    }
    public static class ExcelRangeSizes
    {
        public static ExcelRangeSize Default = new ExcelRangeSize(1, 1);
    }
}
