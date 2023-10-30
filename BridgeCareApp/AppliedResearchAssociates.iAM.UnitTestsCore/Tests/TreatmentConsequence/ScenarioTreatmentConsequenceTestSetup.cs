using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class ScenarioTreatmentConsequenceTestSetup
    {
        public static TreatmentConsequenceDTO ModelForEntityInDb(
            IUnitOfWork unitOfWork,
            Guid simulationId,
            Guid treatmentId,
            Guid? id = null,
            string attribute = "attribute",
            string equation = null,
            string criterion = null
            )
        {
            var consequence = TreatmentConsequenceDtos.Dto(id, attribute, equation, criterion);
            var costList = new List<TreatmentConsequenceDTO> { consequence };
            var dictionary = new Dictionary<Guid, List<TreatmentConsequenceDTO>> { { treatmentId, costList } };
            unitOfWork.TreatmentConsequenceRepo.UpsertOrDeleteScenarioTreatmentConsequences(dictionary, simulationId);
            return consequence;
        }
    }
}
