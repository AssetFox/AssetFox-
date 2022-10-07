using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces;
using Moq;

namespace BridgeCareCoreTests.Helpers
{
    public static class ExpressionValidationServiceMocks
    {
        public static Mock<IExpressionValidationService> New()
        {
            var mock = new Mock<IExpressionValidationService>();
            return mock;
        }
    }
}
