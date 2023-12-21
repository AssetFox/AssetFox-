using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class EquationTestSetup
    {
        public static EquationDTO Dto(string equation, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new EquationDTO
            {
                Expression = equation,
                Id = resolveId,
            };
            return dto;
        }
    }
}
