namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class TextAttribute : Attribute<string>
{
    internal TextAttribute(string name) : base(name)
    {
    }
}
