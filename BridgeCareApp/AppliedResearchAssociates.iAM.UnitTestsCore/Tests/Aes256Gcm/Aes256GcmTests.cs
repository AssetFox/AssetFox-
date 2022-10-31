using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class Aes256GcmTests
    {
        public const string CypherText = "Fj6y4slHYqm2PAw/MyIepQLXxHU87hg87svgP5MaqwxQqLtZBmDi1f5rHyj0s35LNWCELke1cmb2p9iV/GyQxjxsNzbfHKyZdI5m5HlSiMWEihoS1aoFnKoiMUDrb8mD6+B+lXFQ5e/G3SqUvgRTLfQTjBoQ1CvEnklT7SnqWvJBB6sVXdXhcYiBQWjkCzXFHDVMPueOTFn6eiZd/8QE+Uwk6smc7hIihQb+OxcpYiZ7Qoy/NtXowmgI/IkJOjaJklo28B3zAg==";
        public const string Key = "7x!z%C*F-JaNdRgUk242s5v8y,B?D(G.";

        [Fact]
        public void Decrypt_NullExpected_IsNull()
        {
            var keyBytes = Encoding.UTF8.GetBytes(Key);
            var decrypted = AES256GCM.Decrypt(CypherText, keyBytes);
            Assert.Null(decrypted);
        }
    }
}
