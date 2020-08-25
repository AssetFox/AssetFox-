using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Segmentation;

namespace AppliedResearchAssociates.iAM.UnitTests
{
    public static class TestDataForSegmentation
    {
        public static Segment NumericSegmentLinearLocation = new Segment
            (TestDataForAttribute.NumericAttributeDataLinearLocation.Location, TestDataForAttribute.NumericAttributeDataLinearLocation);

        public static Segment TextSegmentLinearLocation = new Segment
            (TestDataForAttribute.TextAttributeDataLinearLocOutput.Location, TestDataForAttribute.TextAttributeDataLinearLocOutput);
    }
}
