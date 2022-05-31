using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace AppliedResearchAssociates.iAM.TestHelpers
{
    public static class ObjectAssertions
    {
        public static void Equivalent(object o1, object o2)
        {
            o1.Should().BeEquivalentTo(o2);
        }
    }
}
