using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AppliedResearchAssociates.iAM.Analysis.Testing
{
    public class NetworkHistoryOnDiskTests
    {
        [Test]
        public void BadGet()
        {
            using NetworkHistoryOnDisk history = new();
            var accessor = history.GetAccessor();
            var success = accessor.TryGetValue("foo", "bar", 2022, out double value);
            Assert.That(success, Is.False);
        }

        [Test]
        public void CreateAndDispose()
        {
            using NetworkHistoryOnDisk history = new();
        }

        [Test]
        public void GetYears()
        {
            using NetworkHistoryOnDisk history = new();
            var accessor = history.GetAccessor();
            accessor.SetValue("foo", "bar", 2022, 42d);
            accessor.SetValue("foo", "baz", 2022, Math.PI);
            accessor.SetValue("foo", "bar", 2021, 4.2);
            var years = accessor.GetYears<double>("foo", "bar");
            Assert.That(years, Is.EquivalentTo(new KeyValuePair<int, double>[] { new(2021, 4.2), new(2022, 42d) }));
        }

        [Test]
        public void GetYearsFromEmpty()
        {
            using NetworkHistoryOnDisk history = new();
            var accessor = history.GetAccessor();
            var years = accessor.GetYears<double>("foo", "bar");
            Assert.That(years, Is.Empty);
        }

        [Test]
        public void SetAndGet()
        {
            using NetworkHistoryOnDisk history = new();
            var accessor = history.GetAccessor();
            accessor.SetValue("foo", "bar", 2022, 42d);
            var success = accessor.TryGetValue("foo", "bar", 2022, out double value);
            Assert.That(success, Is.True);
            Assert.That(value, Is.EqualTo(42));
        }

        [Test]
        public void TwoReaders()
        {
            using NetworkHistoryOnDisk history = new();
            var writer = history.GetAccessor();
            var reader1 = history.GetAccessor();
            var reader2 = history.GetAccessor();
            writer.SetValue("foo", "bar", 2022, 0.1);
            writer.SetValue("foo", "baz", 2022, "fantastic");
            var success1 = reader1.TryGetValue("foo", "baz", 2022, out string value1);
            var success2 = reader2.TryGetValue("foo", "bar", 2022, out double value2);
            Assert.That(success1, Is.True);
            Assert.That(success2, Is.True);
            Assert.That(value1, Is.EqualTo("fantastic"));
            Assert.That(value2, Is.EqualTo(0.1));
        }

        [Test]
        public void TwoWriters()
        {
            using NetworkHistoryOnDisk history = new();
            var writer1 = history.GetAccessor();
            var writer2 = history.GetAccessor();
            var reader = history.GetAccessor();
            writer1.SetValue("foo", "bar", 2022, 0.1);
            writer2.SetValue("foo", "baz", 2022, "fantastic");
            var success = reader.TryGetValue("foo", "baz", 2022, out string value);
            Assert.That(success, Is.True);
            Assert.That(value, Is.EqualTo("fantastic"));
        }
    }
}
