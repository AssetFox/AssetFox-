using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models.DefaultData;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class InvestmentDefaultDataServiceMocks
    {
        public static Mock<IInvestmentDefaultDataService> New(InvestmentDefaultData? defaultData = null)
        {
            var resolveDefaultData = defaultData ?? new InvestmentDefaultData();
            var mock = new Mock<IInvestmentDefaultDataService>();
            mock.Setup(m => m.GetInvestmentDefaultData()).Returns(Task.FromResult(resolveDefaultData));
            return mock;
        }
    }
}
