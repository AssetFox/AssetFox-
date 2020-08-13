namespace AppliedResearchAssociates.iAM
{
    public interface ISection
    {
        double Area { get; }

        double GetAttributeValue(string attributeName);
    }
}
