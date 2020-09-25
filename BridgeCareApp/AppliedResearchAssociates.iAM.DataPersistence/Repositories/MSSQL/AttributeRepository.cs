using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL
{
    public class AttributeRepository : GenericMSSQLRepository<Attribute>
    {
        public AttributeRepository(IAMContext context) : base(context) {}
    }
}
