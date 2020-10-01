using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : MSSQLRepository<Attribute, AttributeEntity>
    {
        public AttributeRepository(IAMContext context) : base(context) {}

        protected override AttributeEntity ToDataEntity(Attribute domainModel)
        {
            throw new NotImplementedException();
        }

        protected override Attribute ToDomainModel(AttributeEntity dataEntity)
        {
            throw new NotImplementedException();
        }
    }
}
