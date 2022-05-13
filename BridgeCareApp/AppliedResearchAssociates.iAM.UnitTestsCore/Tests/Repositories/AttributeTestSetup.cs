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
        public static NumericAttribute Numeric(Guid? id = null)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var randomName = RandomStrings.Length11();
            var attribute = new NumericAttribute(2, 3, 1, resolvedId, randomName, "Number", "command", DataMiner.ConnectionType.MSSQL, "", true, false);
            return attribute;
        }

        public static TextAttribute Text(Guid? id = null)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var randomName = RandomStrings.Length11();
            var attribute = new TextAttribute("defaultValue", resolvedId, randomName, "ruleType", "command", DataMiner.ConnectionType.MSSQL, "", false, true);
            return attribute;
        }
    }
}
