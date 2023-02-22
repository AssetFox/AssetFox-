using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class AttributeSystem
    {
        public List<CalculatedField> CalculatedFields { get; set; }

        public List<NumberAttribute> NumberAttributes { get; set; }

        public List<TextAttribute> TextAttributes { get; set; }
    }
}
