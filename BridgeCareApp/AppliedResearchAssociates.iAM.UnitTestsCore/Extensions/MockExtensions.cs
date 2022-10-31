using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Extensions
{
    public static class MockExtensions
    {
        public static List<IInvocation> InvocationsWithName<T>(
            this Mock<T> mock, string methodName)
            where T: class
        {
            var allInvocations = mock.Invocations.ToList();
            var invocationsWithName = allInvocations.Where(i => i.Method.Name == methodName).ToList();
            return invocationsWithName;
        }

        public static IInvocation SingleInvocationWithName<T>(
            this Mock<T> mock, string methodName)
            where T: class
        {
            var invocations = mock.InvocationsWithName(methodName);
            var invocation = invocations.Single();
            return invocation;
        }
    }
}
