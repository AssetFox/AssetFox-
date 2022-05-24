using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Debugging
{
    public class ErrorConditionIncrement : IDisposable
    {
        private string CallerName { get; set; }
        public ErrorConditionIncrement([CallerMemberName] string callerName = "")
        {
            ErrorCondition.ErrorCallerMembers.AddOrUpdate(
                callerName,
                1,
                (s, n) => n + 1);
            CallerName = callerName;
            Interlocked.Increment(ref ErrorCondition.ErrorConditionCount);
        }
        public void Dispose()
        {
            Interlocked.Decrement(ref ErrorCondition.ErrorConditionCount);
            var didRemove = false;
            while (!didRemove)
            {
                if (ErrorCondition.ErrorCallerMembers.TryRemove(CallerName, out int value))
                {
                    didRemove = true;
                    if (value > 0)
                    {
                        ErrorCondition.ErrorCallerMembers.AddOrUpdate(CallerName, value, (s, n) => n + value);
                    }
                }
            }
        }
    }
}
