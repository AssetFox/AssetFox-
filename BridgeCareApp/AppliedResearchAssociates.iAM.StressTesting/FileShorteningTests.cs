using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using Newtonsoft.Json;
using Xunit;

namespace AppliedResearchAssociates.iAM.StressTesting
{
    public class FileShorteningTests
    {
        private void ShortenFile(string inputFile, string outputFile, int assetTake)
        {
            var input = FileReader.ReadAllTextInGitIgnoredFile(inputFile);
            var deserialized = JsonConvert.DeserializeObject<SimulationOutput>(input);
            SimulationOutputTruncator.Truncate(deserialized, assetTake);
            var output = JsonConvert.SerializeObject(deserialized);
            FileReader.WriteTextToGitIgnoredFile(outputFile, output);
        }


        [Fact]
        public void ShortenTo1()
        {
            ShortenFile(CannedSimulationOutput.Filename, CannedSimulationOutput.Filename1, 1);
        }

        [Fact]
        public void ShortenTo2()
        {
            ShortenFile(CannedSimulationOutput.Filename, CannedSimulationOutput.Filename2, 2);
        }

        [Fact]
        public void ShortenTo10()
        {
            ShortenFile(CannedSimulationOutput.Filename, CannedSimulationOutput.Filename10, 10);
        }

        [Fact]
        public void ShortenTo100()
        {
            ShortenFile(CannedSimulationOutput.Filename, CannedSimulationOutput.Filename100, 100);
        }
        [Fact]
        public void ShortenTo600()
        {
            ShortenFile(CannedSimulationOutput.Filename, CannedSimulationOutput.Filename600, 600);
        }
    }
}
