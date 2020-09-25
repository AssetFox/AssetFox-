using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistence.Models
{
    public class Attribute
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public string ConnectionString { get; set; }
    }
}
