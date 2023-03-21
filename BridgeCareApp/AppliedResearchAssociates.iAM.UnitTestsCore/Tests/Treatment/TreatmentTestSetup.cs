﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentTestSetup
    {
        public static TreatmentDTO ModelForSingleTreatmentOfLibraryInDb(IUnitOfWork unitOfWork, Guid treatmentLibraryId, Guid? id = null, string name = "Treatment name")
        {
            var dto = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(id, name);
            var dtos = new List<TreatmentDTO> { dto };
            unitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dtos, treatmentLibraryId);
            return dto;
        }
    }
}