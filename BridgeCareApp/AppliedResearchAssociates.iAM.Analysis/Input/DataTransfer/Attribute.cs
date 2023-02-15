namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{

    public class Attribute
    {
        public string ID { get; set; }
    }

    public class Attribute<T> : Attribute
    {
        public T DefaultValue { get; set; }
    }
}
