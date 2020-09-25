namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Models
{
    public class AttributeMetaDatum
    {
        public string AttributeName { get; set; }
        public string DefaultValue { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public string Location { get; set; }
        public string DataType { get; set; }
        public string ConnectionString { get; set; }
        public string DataSource { get; set; }
        public string DataRetrievalCommand { get; set; }
        public string AggregationRule { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsAscending { get; set; }
    }
}
