﻿using System;
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

        private readonly ICriterionLibraryRepository _criterionLibraryRepo;
        private readonly IAMContext _context;

        public TreatmentSupersessionRepository(ICriterionLibraryRepository criterionLibraryRepo, IAMContext context)
        {
            _criterionLibraryRepo =
                criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

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
                            supersessionEntityIdsPerExpression.Add(__.Criterion.Expression, new List<Guid>{ entity.Id });
                        }
                    }

                    return entity;
                }))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _context.TreatmentSupersession.AddRange(supersessionEntities);
            }
            else
            {
                _context.BulkInsert(supersessionEntities);
            }

            if (supersessionEntityIdsPerExpression.Values.Any())
            {
                _criterionLibraryRepo.JoinEntitiesWithCriteria(supersessionEntityIdsPerExpression, "TreatmentSupersessionEntity", simulationName);
            }
        }
    }
}
