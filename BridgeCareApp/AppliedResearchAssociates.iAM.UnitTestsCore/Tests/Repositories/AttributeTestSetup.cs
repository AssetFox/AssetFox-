using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories
{
    public static class AttributeTestSetup
    {
        public static NumericAttribute Numeric(Guid? id = null, string name = null)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var randomName = name ?? RandomStrings.Length11();
            var attribute = new NumericAttribute(2, 3, 1, resolvedId, randomName, "AVERAGE", "Command", DataMiner.ConnectionType.MSSQL, "connectionString", true, false);
            return attribute;
        }

        public static TextAttribute Text(Guid? id = null)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var randomName = RandomStrings.Length11();
            var attribute = new TextAttribute("defaultValue", resolvedId, randomName, "PREDOMINANT", "command", DataMiner.ConnectionType.MSSQL, "connectionString", false, true);
            return attribute;
        }
    }
}
