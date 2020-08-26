using NUnit.Framework;

namespace AppliedResearchAssociates.Testing
{
    public class CycleDetectionTests
    {
        [Test]
        public void ClrsFigure22_4()
        {
            // https://stackoverflow.com/a/53995651/402749

            var u = new Vertex();
            var v = new Vertex();
            var w = new Vertex();
            var x = new Vertex();
            var y = new Vertex();
            var z = new Vertex();

            u.Name = nameof(u);
            v.Name = nameof(v);
            w.Name = nameof(w);
            x.Name = nameof(x);
            y.Name = nameof(y);
            z.Name = nameof(z);

            u.Adjacencies = new[] { v, x };
            v.Adjacencies = new[] { y };
            w.Adjacencies = new[] { y, z };
            x.Adjacencies = new[] { v };
            y.Adjacencies = new[] { x };
            z.Adjacencies = new[] { z };

            var vertices = new[] { u, w };
            var cycles = CycleDetection.FindAllCycles(vertices, vertex => vertex.Adjacencies);

            Assert.That(cycles, Is.Not.Empty);
            Assert.That(cycles, Does.Contain(new[] { v, y, x }).Using(SequenceEqualityComparer.Default));
            Assert.That(cycles, Does.Contain(new[] { z }).Using(SequenceEqualityComparer.Default));
        }

        private sealed class Vertex
        {
            public Vertex[] Adjacencies;

            public string Name;

            public override string ToString() => Name;
        }
    }
}
