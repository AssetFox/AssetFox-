using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DataSources;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
{
    public static class TreatmentCostTestSetup
    {
        public static TreatmentCostDTO ModelForEntityInDb(IUnitOfWork unitOfWork, Guid treatmentId, Guid treatmentLibraryId, Guid? id = null, string mergedCriteriaExpression = null)
        {
            var cost = TreatmentCostDtos.WithEquationAndCriterionLibrary(id, null, null, "12345", mergedCriteriaExpression);
            var costList = new List<TreatmentCostDTO> { cost };
            var dictionary = new Dictionary<Guid, List<TreatmentCostDTO>> { { treatmentId, costList } };
            unitOfWork.TreatmentCostRepo.UpsertOrDeleteTreatmentCosts(dictionary, treatmentLibraryId);
            return cost;
        }
    }
}
