using System;
using System.Collections.Generic;

namespace Test_DbFirst
{
    public partial class Networks
    {
        public Networks()
        {
            Segments = new HashSet<Segments>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Segments> Segments { get; set; }
    }
}
