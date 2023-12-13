using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class OrphanCleanupSprocTests
    {
        private string RunOrphanCleanupSproc()
        {
            var retMessageParam = new SqlParameter("@RetMessage", SqlDbType.VarChar, 250);
            retMessageParam.Direction = ParameterDirection.Output;
            TestHelper.UnitOfWork.Context.Database.ExecuteSqlRaw("EXEC usp_orphan_cleanup @RetMessage", retMessageParam);
            var retMessageValue = retMessageParam.Value;
            return retMessageValue.ToString();
        }

        [Fact]
        public void OrphanCleanup_Runs()
        {
            var retMessage = RunOrphanCleanupSproc();
            Assert.Empty(retMessage);
        }
    }
}
