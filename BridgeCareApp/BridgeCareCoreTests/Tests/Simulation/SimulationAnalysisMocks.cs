using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BridgeCareCore.Interfaces;
using Moq;

namespace BridgeCareCoreTests.Tests.Simulation
{
    public static class SimulationAnalysisMocks
    {
        public static Mock<ISimulationAnalysis> New()
        {
            return new Mock<ISimulationAnalysis>();
        }
    }
}
