using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentSupersessionRepository : ITreatmentSupersessionRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public TreatmentSupersessionRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateTreatmentSupersessions(Dictionary<Guid, List<TreatmentSupersession>> treatmentSupersessionsPerTreatmentId,
            string simulationName)
        {
            var supersessionEntityIdsPerExpression = new Dictionary<string, List<Guid>>();

            var supersessionEntities = treatmentSupersessionsPerTreatmentId.SelectMany(_ =>
                _.Value.Select(__ =>
                {
                    var entity = __.ToEntity(_.Key);

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

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.TreatmentSupersession.AddRange(supersessionEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(supersessionEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

            if (supersessionEntityIdsPerExpression.Values.Any())
            {
                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(
                    supersessionEntityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.TreatmentSupersession, simulationName);
            }
        }
    }
}
