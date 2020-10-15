using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.LiteDb.Entities
{
    public class GarbageEntity<T>
    {
        public GarbageEntity()
        {

        }

        public AttributeEntity AttributeEntity { get; set; }
        public int Year { get; set; }
        public T Value { get; set; }
    }
}
