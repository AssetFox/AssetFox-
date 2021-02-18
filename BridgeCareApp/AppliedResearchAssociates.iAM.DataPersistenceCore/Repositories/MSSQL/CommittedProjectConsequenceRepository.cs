using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CommittedProjectConsequenceRepository : ICommittedProjectConsequenceRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public CommittedProjectConsequenceRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateCommittedProjectConsequences(Dictionary<Guid, List<(Guid attributeId, TreatmentConsequence consequence)>> consequencePerAttributeIdPerProjectId)
        {
            var committedProjectConsequenceEntities = consequencePerAttributeIdPerProjectId
                .SelectMany(_ => _.Value.Select(__ => __.consequence.ToEntity(_.Key, __.attributeId)))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.CommittedProjectConsequence.AddRange(committedProjectConsequenceEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(committedProjectConsequenceEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
