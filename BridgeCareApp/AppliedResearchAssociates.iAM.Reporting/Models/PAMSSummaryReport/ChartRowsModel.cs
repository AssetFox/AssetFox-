namespace AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport
{
    public class ChartConditionModel
    {
        public int sourceStartRow { get; set; }
    }

    //IRI Models
    public class IRI_BPN_1_ChartModel : ChartConditionModel { }
    public class IRI_BPN_2_ChartModel : ChartConditionModel { }
    public class IRI_BPN_3_ChartModel : ChartConditionModel { }
    public class IRI_BPN_4_ChartModel : ChartConditionModel { }
    public class IRI_StateWide_ChartModel : ChartConditionModel { }

    //OPI Models
    public class OPI_BPN_1_ChartModel : ChartConditionModel { }
    public class OPI_BPN_2_ChartModel : ChartConditionModel { }
    public class OPI_BPN_3_ChartModel : ChartConditionModel { }
    public class OPI_BPN_4_ChartModel : ChartConditionModel { }
    public class OPI_StateWide_ChartModel : ChartConditionModel { }

    public class ChartRowsModel
    {
        public int TotalCountSectionYearsRow { get; set; }
        public int TotalCountBPNYearsRow { get; set; }
                
        public IRI_BPN_1_ChartModel IRI_BPN_1_ChartModel { get; set; }
        public IRI_BPN_2_ChartModel IRI_BPN_2_ChartModel { get; set; }
        public IRI_BPN_3_ChartModel IRI_BPN_3_ChartModel { get; set; }
        public IRI_BPN_4_ChartModel IRI_BPN_4_ChartModel { get; set; }
        public IRI_StateWide_ChartModel IRI_StateWide_ChartModel { get; set; }


        public OPI_BPN_1_ChartModel OPI_BPN_1_ChartModel { get; set; }
        public OPI_BPN_2_ChartModel OPI_BPN_2_ChartModel { get; set; }
        public OPI_BPN_3_ChartModel OPI_BPN_3_ChartModel { get; set; }
        public OPI_BPN_4_ChartModel OPI_BPN_4_ChartModel { get; set; }
        public OPI_StateWide_ChartModel OPI_StateWide_ChartModel { get; set; }


        public ChartRowsModel()
        {
            //IRI
            IRI_BPN_1_ChartModel = new IRI_BPN_1_ChartModel();
            IRI_BPN_2_ChartModel = new IRI_BPN_2_ChartModel();
            IRI_BPN_3_ChartModel = new IRI_BPN_3_ChartModel();
            IRI_BPN_4_ChartModel = new IRI_BPN_4_ChartModel();
            IRI_StateWide_ChartModel = new IRI_StateWide_ChartModel();

            //OPI
            OPI_BPN_1_ChartModel = new OPI_BPN_1_ChartModel();
            OPI_BPN_2_ChartModel = new OPI_BPN_2_ChartModel();
            OPI_BPN_3_ChartModel = new OPI_BPN_3_ChartModel();
            OPI_BPN_4_ChartModel = new OPI_BPN_4_ChartModel();
            OPI_StateWide_ChartModel = new OPI_StateWide_ChartModel();
        }
    }
}
