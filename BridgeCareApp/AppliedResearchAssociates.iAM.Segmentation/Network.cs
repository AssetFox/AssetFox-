using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class Network
    {
        public List<Segment> Segments { get; }
        public Guid Guid { get; }
        public string Name { get; }

        public Network(List<Segment> segments, Guid guid, string name = "")
        {
            Segments = segments;
            Guid = guid;
            Name = name;
        }
    }
}
