using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TreatmentCost
{
    public static class ScenarioTreatmentCostTestSetup
    {
        public static TreatmentCostDTO CostForTreatmentInDb(
            IUnitOfWork unitOfWork,
            Guid treatmentId,
            Guid simulationId,
            Guid? id = null,
            string mergedCriteriaExpression = null)
        {
            var cost = TreatmentCostDtos.WithEquationAndCriterionLibrary(id, equation:"[AGE]");
            var costs = new List<TreatmentCostDTO> { cost };
            var costDictionary = new Dictionary<Guid, List<TreatmentCostDTO>>
            {
                {
                treatmentId,
                costs
                }
            };
            unitOfWork.TreatmentCostRepo.UpsertOrDeleteScenarioTreatmentCosts(
                costDictionary, simulationId);
            return cost;
        }
    }
}
