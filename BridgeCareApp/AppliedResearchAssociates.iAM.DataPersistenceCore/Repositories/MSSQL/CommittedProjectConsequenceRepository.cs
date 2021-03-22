using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CommittedProjectConsequenceRepository : ICommittedProjectConsequenceRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CommittedProjectConsequenceRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateCommittedProjectConsequences(Dictionary<Guid, List<(Guid attributeId, TreatmentConsequence consequence)>> consequencePerAttributeIdPerProjectId)
        {
            var committedProjectConsequenceEntities = consequencePerAttributeIdPerProjectId
                .SelectMany(_ => _.Value.Select(__ => __.consequence.ToEntity(_.Key, __.attributeId)))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.CommittedProjectConsequence.AddRange(committedProjectConsequenceEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsertOrUpdate(committedProjectConsequenceEntities);
            }

            _unitOfWork.Context.SaveChanges();
        }
    }
}
