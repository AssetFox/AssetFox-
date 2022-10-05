using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AppliedResearchAssociates.iAM.Analysis.Testing
{
    public class NetworkHistoryTests
    {
        [Test]
        public void BadGet()
        {
            using NetworkHistory history = new();
            var success = history.TryGetValue("foo", "bar", 2022, out double value);
            Assert.IsFalse(success);
        }

        [Test]
        public void CreateAndDispose()
        {
            using NetworkHistory history = new();
        }

        [Test]
        public void GetYears()
        {
            using NetworkHistory history = new();
            history.SetValue("foo", "bar", 2022, 42d);
            history.SetValue("foo", "baz", 2022, Math.PI);
            history.SetValue("foo", "bar", 2021, 4.2);
            var years = history.GetYears<double>("foo", "bar");
            Assert.That(years, Is.EquivalentTo(new KeyValuePair<int, double>[] { new(2021, 4.2), new(2022, 42d) }));
        }

        [Test]
        public void GetYearsFromEmpty()
        {
            using NetworkHistory history = new();
            var years = history.GetYears<double>("foo", "bar");
            Assert.That(years, Is.Empty);
        }

        [Test]
        public void SetAndGet()
        {
            using NetworkHistory history = new();
            history.SetValue("foo", "bar", 2022, 42d);
            var success = history.TryGetValue("foo", "bar", 2022, out double value);
            Assert.IsTrue(success);
            Assert.That(value, Is.EqualTo(42));
        }
    }
}
