using System;
using System.Collections.Generic;

namespace Test_DbFirst
{
    public partial class Attributes
    {
        public Attributes()
        {
            AttributeDatumEntity = new HashSet<AttributeDatumEntity>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public int ConnectionType { get; set; }

        public virtual ICollection<AttributeDatumEntity> AttributeDatumEntity { get; set; }
    }
}
