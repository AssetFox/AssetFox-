namespace AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport
{
    public class ChartRowsModel
    {
        public int TotalPamsCountSectionYearsRow { get; set; }



        public int TotalPoorPamssCountSectionYearsRow { get; set; }



        public int NHSPamsCountSectionYearsRow { get; set; }

        public int NonNHSPamsCountSectionYearsRow { get; set; }

        public int NHSPamsCountPercentSectionYearsRow { get; set; }

        public int NonNHSPamsCountPercentSectionYearsRow { get; set; }



        public int TotalPamsCountPercentYearsRow { get; set; }


        public int TotalPamsPostedCountByBPNYearsRow { get; set; }


        public int TotalClosedPamsCountByBPNYearsRow { get; internal set; }


        public int TotalPostedAndClosedByBPNYearsRow { get; internal set; }

        public int TotalCashNeededByBPNYearsRow { get; internal set; }
    }
}
