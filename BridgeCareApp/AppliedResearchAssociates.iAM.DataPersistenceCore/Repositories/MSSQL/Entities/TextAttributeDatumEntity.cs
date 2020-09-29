using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TextAttributeDatumEntity : AttributeDatumEntity, IHaveAttributeDatumValue<string>
    {
        public string Value { get; set; }
        public override LocationEntity Location { get; set; }
    }
}
