using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCoreTests.Tests
{
    public static class CalculatedAttributeEquationCriteriaPairDtos
    {
        public static CalculatedAttributeEquationCriteriaPairDTO New(Guid? criterionLibraryId = null, Guid? equationId = null)
        {
            var dto = new CalculatedAttributeEquationCriteriaPairDTO
            {
                Id = Guid.NewGuid(),
                CriteriaLibrary = CriterionLibraryDtos.Dto(criterionLibraryId),
                Equation = EquationDtos.AgePlus1(equationId),
            };
            return dto;
        }
    }
}
