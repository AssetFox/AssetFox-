using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataAssignment.Segmentation
{
    public class Network
    {
        public ICollection<Segment> Segments { get; }
        public Guid Id { get; }
        public string Name { get; set; }

        public Network(ICollection<Segment> segments, Guid id, string name = "")
        {
            Segments = segments;
            Id = id;
            Name = name;
        }
    }
}
