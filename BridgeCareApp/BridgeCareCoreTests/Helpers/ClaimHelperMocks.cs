using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Utils.Interfaces;
using Moq;

namespace BridgeCareCoreTests
{
    public static class ClaimHelperMocks
    {
        public static Mock<IClaimHelper> New()
        {
            var helper = new Mock<IClaimHelper>();
            return helper;
        }
    }
}
