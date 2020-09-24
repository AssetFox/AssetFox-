using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataAccess
{
    public class SegmentationMetaDatum
    {
        public string AttributeName { get; set; }
        public string DefaultValue { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public string ConnectionString { get; set; }
        public string DataSource { get; set; }
        public string DataType { get; set; }
        public string Location { get; set; }
        public string DataRetrievalCommand { get; set; }
        public string AggregationRule { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsAscending { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public ParametersMap ParameterMap { get; set; }
    }
    public class ParametersMap
    {
        public string RouteName { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public string UniqueIdentifier { get; set; }
        public string WellKnownText { get; set; }
        public string Direction { get; set; }
        public string Value { get; set; }
        public DateTime DateTime { get; set; }
    }
}
