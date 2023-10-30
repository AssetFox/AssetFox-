using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment
{
    public class SelectableTreatmentTestSetup
    {
        public static TreatmentDTO CreateTreatmentDtoWithSupersedeRules(string treatmentName, TreatmentDTO preventTreatmentDto)
        {
            var treatmentDto = TreatmentDtos.DtoWithEmptyListsWithCriterionLibrary(Guid.NewGuid(), treatmentName);

            // Add supersede rules
            treatmentDto.SupersedeRules = new List<TreatmentSupersedeRuleDTO> { new TreatmentSupersedeRuleDTO { treatment = preventTreatmentDto, CriterionLibrary = new CriterionLibraryDTO { Id = Guid.NewGuid() }, Id = Guid.NewGuid() } };

            return treatmentDto;
        }        
    }
}
