using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public interface IHaveAttributeDatumValue<T>
    {
        public T Value { get; set; }
    }
}
