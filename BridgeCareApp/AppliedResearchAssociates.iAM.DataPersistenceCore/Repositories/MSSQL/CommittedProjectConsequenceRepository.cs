﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CommittedProjectConsequenceRepository : MSSQLRepository, ICommittedProjectConsequenceRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public CommittedProjectConsequenceRepository(IAMContext context) : base(context) { }

        public void CreateCommittedProjectConsequences(
            List<((Guid committedProjectId, Guid attributeId) committedProjectIdAttributeIdTuple, TreatmentConsequence
                committedProjectConsequence)> committedProjectConsequenceCommittedProjectIdAttributeIdTupleTuple)
        {
            var committedProjectConsequenceEntities = committedProjectConsequenceCommittedProjectIdAttributeIdTupleTuple
                .Select(_ => _.committedProjectConsequence
                    .ToEntity(_.committedProjectIdAttributeIdTuple.committedProjectId, _.committedProjectIdAttributeIdTuple.attributeId))
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.CommittedProjectConsequence.AddRange(committedProjectConsequenceEntities);
            }
            else
            {
                Context.BulkInsert(committedProjectConsequenceEntities);
            }
        }
    }
}
