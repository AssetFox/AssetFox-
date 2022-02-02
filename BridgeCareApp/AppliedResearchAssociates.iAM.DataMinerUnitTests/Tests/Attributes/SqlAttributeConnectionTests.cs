using Xunit;
using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using Moq;
using System.Linq;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
{
    public class SqlAttributeConnectionTests
    {
        private Mock<Attribute> mockAttribute;
        private readonly string testConnection = "data source=localhost;initial catalog=IAMv2;persist security info=True;user id=sqluser;password=su1Local*;MultipleActiveResultSets=True;App=EntityFramework";
        private string testCommand = string.Empty;         

        [Fact]
        public void GetDataWithStringTest()
        {
            Init("NUMBER", "Name");
            var sqlAttributeConnection = new SqlAttributeConnection(mockAttribute.Object);
            var result = sqlAttributeConnection.GetData<string>();
            Assert.NotNull(result);
            var resultElements = result.ToList();            
            Assert.Single(resultElements);
            Assert.IsType<AttributeDatum<string>>(resultElements[0]);
        }

        [Fact]
        public void GetDataWithNumberTest()
        {
            Init("STRING", "ConnectionType");
            var sqlAttributeConnection = new SqlAttributeConnection(mockAttribute.Object);
            var result = sqlAttributeConnection.GetData<double>();

            Assert.NotNull(result);
            var resultElements = result.ToList();            
            Assert.Single(resultElements);
            Assert.IsType<AttributeDatum<double>>(resultElements[0]);
        }

        private void Init(string type, string dataColumn)
        {
            testCommand = "SELECT Top 1 Id AS ID_, Name AS FACILITY, Name AS SECTION, CreatedDate AS DATE_, " + dataColumn + " AS DATA_ FROM dbo.Attribute";
            mockAttribute = new Mock<Attribute>(Guid.Empty, "Test", type, "TestRuleType", testCommand, DataMiner.ConnectionType.MSSQL, testConnection, false, false);
        }
    }
}
