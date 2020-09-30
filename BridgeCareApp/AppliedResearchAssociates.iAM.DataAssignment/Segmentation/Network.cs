using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataAssignment.Segmentation
{
    public class Network
    {
        public ICollection<Segment> Segments { get; }
        public Guid Guid { get; }
        public string Name { get; }

        public Network(ICollection<Segment> segments, Guid guid, string name = "")
        {
            Segments = segments;
            Guid = guid;
            Name = name;
        }
    }
}
