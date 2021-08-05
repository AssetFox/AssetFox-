using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentSupersessionRepository : ITreatmentSupersessionRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentSupersessionRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateTreatmentSupersessions(Dictionary<Guid, List<TreatmentSupersession>> treatmentSupersessionsPerTreatmentId,
            string simulationName)
        {
            var supersessionEntityIdsPerExpression = new Dictionary<string, List<Guid>>();

            var supersessionEntities = treatmentSupersessionsPerTreatmentId.SelectMany(_ =>
                _.Value.Select(__ =>
                {
                    var entity = __.ToScenarioEntity(_.Key);

                    if (!__.Criterion.ExpressionIsBlank)
                    {
                        if (supersessionEntityIdsPerExpression.ContainsKey(__.Criterion.Expression))
                        {
                            supersessionEntityIdsPerExpression[__.Criterion.Expression].Add(entity.Id);
                        }
                        else
                        {
                            supersessionEntityIdsPerExpression.Add(__.Criterion.Expression, new List<Guid> { entity.Id });
                        }
                    }

                    return entity;
                }))
                .ToList();

            _unitOfWork.Context.AddAll(supersessionEntities);

            if (supersessionEntityIdsPerExpression.Values.Any())
            {
                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(
                    supersessionEntityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.TreatmentSupersession, simulationName);
            }
        }
    }
}
