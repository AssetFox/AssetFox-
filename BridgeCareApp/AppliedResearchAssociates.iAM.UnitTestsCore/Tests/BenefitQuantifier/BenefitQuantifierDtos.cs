using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.BenefitQuantifier
{
    public static class BenefitQuantifierDtos
    {
        public static BenefitQuantifierDTO Dto(Guid networkId, Guid? equationId = null)
        {
            var equation = EquationDtos.AgePlus1(equationId);
            var dto = new BenefitQuantifierDTO
            {
                NetworkId = networkId,
                Equation = equation,
            };
            return dto;
        }
    }
}
