namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Models
{
    public class AggregateDataSegment
    {
        public AggregateDataSegment(Segment segment) => Segment = segment;
        public Segment Segment { get; set; }
    }
}
