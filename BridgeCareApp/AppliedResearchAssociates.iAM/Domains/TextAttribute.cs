﻿namespace AppliedResearchAssociates.iAM.Domains
{
    public sealed class TextAttribute : Attribute<string>
    {
        public TextAttribute(string name) : base(name)
        {
        }
    }
}