namespace BridgeCareCore.Models.SummaryReport
{
    public class YearsData
    {
        public int Year { get; set; }

        public string Treatment { get; set; }

        public double Amount { get; set; }

        public bool isCommitted { get; set; } = false;
    }
}
