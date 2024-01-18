using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils.DatabaseRecordCounting
{
    public class RowCountReturnCustomEntity
    {
        public string TableName { get; set; }
        public int Records { get; set; }

        public override string ToString() => $@"{Records} {TableName}";
    }
}
