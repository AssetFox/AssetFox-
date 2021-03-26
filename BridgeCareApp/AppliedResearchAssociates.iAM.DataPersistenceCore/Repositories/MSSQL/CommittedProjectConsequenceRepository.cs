using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CommittedProjectConsequenceRepository : ICommittedProjectConsequenceRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CommittedProjectConsequenceRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateCommittedProjectConsequences(Dictionary<Guid, List<(Guid attributeId, TreatmentConsequence consequence)>> consequencePerAttributeIdPerProjectId)
        {
            var committedProjectConsequenceEntities = consequencePerAttributeIdPerProjectId
                .SelectMany(_ => _.Value.Select(__ => __.consequence.ToEntity(_.Key, __.attributeId)))
                .ToList();

            _unitOfWork.Context.AddAll(committedProjectConsequenceEntities);
        }
    }
}
