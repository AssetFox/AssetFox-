using System;
using System.Collections.Generic;

namespace Test_DbFirst
{
    public partial class Routes
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid LinearLocationId { get; set; }
        public string Discriminator { get; set; }
        public int? Direction { get; set; }

        public virtual Locations LinearLocation { get; set; }
    }
}
