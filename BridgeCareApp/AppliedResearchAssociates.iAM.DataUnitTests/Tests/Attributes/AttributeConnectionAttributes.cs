using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataUnitTests.TestUtils;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests
{
    public static class AttributeConnectionAttributes
    {
        public static TextAttribute String(string connectionString, Guid dataSourceId)
        {
            var columnName = CommonTestParameterValues.NameColumn;
            var testCommand = "SELECT Top 1 Id AS ID_, Name AS FACILITY, Name AS SECTION, Name AS LOCATION_IDENTIFIER, CreatedDate AS DATE_, " + columnName + " AS DATA_ FROM dbo.Attribute";
            var attributeName = RandomStrings.WithPrefix("TextAttribute");
            var returnValue = new TextAttribute(
                attributeName,
                Guid.Empty,
                CommonTestParameterValues.Name,
                AggregationRuleTypeNames.Predominant,
                testCommand,
                Data.ConnectionType.MSSQL,
                connectionString,
                false,
                false,
                dataSourceId);
            return returnValue;
        }

        public static TextAttribute ForExcelTestData(Guid dataSourceId)
        {
            var columnName = CommonTestParameterValues.NameColumn;
            var testCommand = "DISTRICT";
            var returnValue = new TextAttribute(
                "TextAttribute",
                Guid.NewGuid(),
                "DISTRICT",
                AggregationRuleTypeNames.Predominant,
                testCommand,
                Data.ConnectionType.EXCEL,
                "",
                false,
                false,
                dataSourceId);
            return returnValue;
        }
    }
}
