using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Debugging
{
    public class ErrorConditionIncrement : IDisposable
    {
        public ErrorConditionIncrement()
        {
            Interlocked.Increment(ref ErrorCondition.ErrorConditionCount);
        }
        public void Dispose()
        {
            Interlocked.Decrement(ref ErrorCondition.ErrorConditionCount);
        }
    }
}
