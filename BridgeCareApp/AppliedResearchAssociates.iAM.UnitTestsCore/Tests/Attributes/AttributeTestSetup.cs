using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes
{
    public static class UnitTestsCoreAttributeTestSetup
    {
        public static void EnsureAttributeExists(AttributeDTO dto)
        {
            var testHelper = TestHelper.Instance;
            var existingAttribute = testHelper.UnitOfWork.AttributeRepo.GetSingleByName(dto.Name);
            if (existingAttribute == null)
            {
                testHelper.UnitOfWork.AttributeRepo.UpsertAttributes(dto);
            }
        }

        public static AttributeDTO ExcelAttributeForEntityInDb(BaseDataSourceDTO dataSourceDTO)
        {
            var attribute = AttributeTestSetup.NumericDto(dataSourceDTO, connectionType: Data.ConnectionType.EXCEL);
            EnsureAttributeExists(attribute);
            return attribute;
        }
    }
}
