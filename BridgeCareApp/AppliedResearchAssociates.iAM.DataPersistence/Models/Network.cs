using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistence.Models
{
    public class Network
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Segment> Segments { get; set; }
    }
}
