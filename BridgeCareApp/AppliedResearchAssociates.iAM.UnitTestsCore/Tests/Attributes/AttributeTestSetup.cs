using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes
{
    public static class AttributeTestSetup
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
    }
}
