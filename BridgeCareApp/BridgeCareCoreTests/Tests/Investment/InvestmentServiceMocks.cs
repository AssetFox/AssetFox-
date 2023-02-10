using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class InvestmentBudgetServiceMocks { 
  
        public static Mock<IInvestmentBudgetsService> New()
        {
            var mock = new Mock<IInvestmentBudgetsService>();
            return mock;
        }
    }
}
