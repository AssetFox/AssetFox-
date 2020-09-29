using System;
using System.Collections.Generic;

namespace Test_DbFirst
{
    public partial class Segments
    {
        public Segments()
        {
            AttributeDatumEntity = new HashSet<AttributeDatumEntity>();
        }

        public Guid Id { get; set; }
        public Guid NetworkId { get; set; }

        public virtual Networks Network { get; set; }
        public virtual Locations Locations { get; set; }
        public virtual ICollection<AttributeDatumEntity> AttributeDatumEntity { get; set; }
    }
}
