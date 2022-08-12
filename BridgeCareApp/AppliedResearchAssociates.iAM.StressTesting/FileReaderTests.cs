using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppliedResearchAssociates.iAM.StressTesting
{
    public class FileReaderTests
    {
        [Fact]
        public void ReadSimulationOutput_Does()
        {
            var text = FileReader.ReadAllTextInGitIgnoredFile("SimulationOutput.json");
        }
    }
}
