using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository<Attribute>
    {
        public AttributeRepository(IAMContext context) : base(context) {}
    }
}
