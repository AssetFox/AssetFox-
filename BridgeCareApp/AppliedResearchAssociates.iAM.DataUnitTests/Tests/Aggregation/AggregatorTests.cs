using Xunit;
using System;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.DataUnitTests.TestUtils;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests.Aggregation
{
    public class AggregatorTests
    {
        List<IAttributeDatum> attributeData;
        List<MaintainableAsset> maintainableAssets = new List<MaintainableAsset>();
        private readonly Guid guId = Guid.Empty;
        private readonly SectionLocation sectionLocation1;
        private readonly SectionLocation sectionLocation2;

        public AggregatorTests()
        {
            attributeData = new List<IAttributeDatum>();
            sectionLocation1 = new SectionLocation(guId, CommonTestParameterValues.LocationIdentifier1);
            sectionLocation2 = new SectionLocation(guId, CommonTestParameterValues.LocationIdentifier2);
        }
    }
}
