using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates
{
    public static class CycleDetection
    {
        public static List<List<T>> FindAllCycles<T>(
            IEnumerable<T> vertexValues,
            Func<T, IEnumerable<T>> getAdjacentValues,
            IEqualityComparer<T> equalityComparer = null)
        {
            equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

            var vertices = vertexValues
                .Distinct(equalityComparer)
                .Select(value => new Vertex<T>(value))
                .ToList();

            var indexPerValue = vertices
                .Select(ValueTuple.Create<Vertex<T>, int>)
                .ToDictionary(_ => _.Item1.Value, _ => _.Item2, equalityComparer);

            var cycles = new List<List<T>>();

            for (var i = 0; i < vertices.Count; ++i)
            {
                var u = vertices[i];

                if (u.Status == Status.Undiscovered)
                {
                    dfsVisit(u);
                }
            }

            return cycles;

            void dfsVisit(Vertex<T> u)
            {
                u.Status = Status.Discovered;

                foreach (var v_Value in getAdjacentValues(u.Value))
                {
                    Vertex<T> v;

                    if (indexPerValue.TryGetValue(v_Value, out var i))
                    {
                        v = vertices[i];
                    }
                    else
                    {
                        i = vertices.Count;
                        indexPerValue.Add(v_Value, i);

                        v = new Vertex<T>(v_Value);
                        vertices.Add(v);
                    }

                    if (v.Status == Status.Undiscovered)
                    {
                        v.Parent = u;
                        dfsVisit(v);
                    }
                    else if (v.Status == Status.Discovered)
                    {
                        var cycle = new List<T>();

                        for (var w = u; w != v; w = w.Parent)
                        {
                            cycle.Add(w.Value);
                        }

                        cycle.Add(v.Value);
                        cycle.Reverse();

                        cycles.Add(cycle);
                    }
                }

                u.Status = Status.Explored;
            }
        }

        private enum Status
        {
            Undiscovered,
            Discovered,
            Explored,
        }

        private sealed class Vertex<T>
        {
            public Vertex(T value) => Value = value;

            public Vertex<T> Parent { get; set; } = null;

            public Status Status { get; set; } = Status.Undiscovered;

            public T Value { get; }
        }
    }
}
