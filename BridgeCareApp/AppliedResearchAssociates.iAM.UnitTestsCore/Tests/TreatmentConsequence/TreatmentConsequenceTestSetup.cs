using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentConsequenceTestSetup
    {
        public static TreatmentConsequenceDTO ModelForEntityInDb(
            IUnitOfWork unitOfWork,
            Guid treatmentLibraryId,
            Guid treatmentId,
            Guid? id
            )
        {
            var consequence = TreatmentConsequenceDtos.Dto(id);
            var consequences = new List<TreatmentConsequenceDTO> { consequence };
            var dictionary = new Dictionary<Guid, List<TreatmentConsequenceDTO>>{
                { treatmentId, consequences }
            };

            unitOfWork.TreatmentConsequenceRepo.UpsertOrDeleteTreatmentConsequences(
                dictionary,
                treatmentLibraryId);

            return consequence;
        }
    }
}
