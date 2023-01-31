using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCoreTests.Tests
{
    public static class AggregatedResultDtos
    {
        public static AggregatedResultDTO Text(AbbreviatedAttributeDTO attribute, Guid assetId, string text)
            => new()
            {
                Attribute = attribute,
                MaintainableAssetId = assetId,
                TextValue = text
            };
        public static AggregatedResultDTO Number(AbbreviatedAttributeDTO attribute, Guid assetId, double number)
            => new()
            {
                Attribute = attribute,
                MaintainableAssetId = assetId,
                NumericValue = number,
            };
    }
}
