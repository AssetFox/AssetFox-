using NUnit.Framework;

namespace AppliedResearchAssociates.iAM.Analysis.Testing
{
    public class NetworkHistoryTests
    {
        [Test]
        public void CreateAndDispose()
        {
            using NetworkHistory history = new();
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
