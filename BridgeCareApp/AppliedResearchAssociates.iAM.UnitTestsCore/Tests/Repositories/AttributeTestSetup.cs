using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories
{
    public static class AttributeTestSetup
    {
        public static NumericAttribute Numeric(Guid? id = null, string name = null, Guid? dataSourceId = null, ConnectionType connectionType = ConnectionType.MSSQL)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var randomName = name ?? RandomStrings.Length11();
            var attribute = new NumericAttribute(2, 3, 1, resolvedId, randomName, "AVERAGE", "Command", connectionType, "connectionString", true, false, dataSourceId);            return attribute;
        }

        public static AttributeDTO NumericDto(BaseDataSourceDTO dataSourceDTO, Guid? id = null, string name = null, ConnectionType connectionType = ConnectionType.MSSQL)
        {
            var attribute = Numeric(id, name, dataSourceDTO.Id, connectionType);
            var dto = AttributeMapper.ToDto(attribute, dataSourceDTO);
            return dto;
        }

        public static TextAttribute Text(Guid? id = null, bool calculated = false)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var randomName = RandomStrings.Length11();
            var attribute = new TextAttribute("defaultValue", resolvedId, randomName, "PREDOMINANT", "command", Data.ConnectionType.MSSQL, "connectionString", calculated, true, Guid.Empty);
            return attribute;
        }
    }
}
