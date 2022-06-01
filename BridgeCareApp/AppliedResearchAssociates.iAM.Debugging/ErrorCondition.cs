using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Debugging
{
    /* We currently have tests that fail only when they are run in 
     * conjunction with certain other tests. To debug that
     * while retaining one's sanity, it is pretty much mandatory to have something like this class. The problem
     * is that one needs to be able to set breakpoints that only fire in
     * the case where some code is about to do the wrong thing. To do that,
     * one needs to be able to say externally that the error condition is about to happen.
     * The code can't be in a test namespace, because the breakpoint itself needs to be
     * callable from non-test code. In general, when a PR is completed, the project containing
     * this class should NOT be a part of any solution. However, the class itself should be keept
     * in source control, I think, because the need for it will sporadically reappear.
     */
    public static class ErrorCondition
    {
        internal static int ErrorConditionCount = 0;
        public static bool ErrorState => ErrorConditionCount > 0;

        /// <summary> convenience method for setting a breakpoint that will fire if and only if we are in an error state</summary>
        public static void ErrorStateBreakpointHolder()
        {
            if (ErrorState)
            {
                int dummyVariable = 666;
            }
        }
    }
}
