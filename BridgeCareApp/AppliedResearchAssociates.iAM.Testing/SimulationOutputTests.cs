using AppliedResearchAssociates.iAM.Analysis;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AppliedResearchAssociates.iAM.Testing
{
    public class SimulationOutputTests
    {
        [Test]
        public void Deserialization() => _ = JsonConvert.DeserializeObject<SimulationOutput>(Properties.Resources.Network_13___Simulation_1181_json);
    }
}
