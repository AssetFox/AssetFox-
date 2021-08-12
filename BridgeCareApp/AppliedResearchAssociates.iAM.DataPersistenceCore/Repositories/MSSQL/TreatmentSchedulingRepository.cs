using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentSchedulingRepository : ITreatmentSchedulingRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentSchedulingRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateTreatmentSchedulings(Dictionary<Guid, List<TreatmentScheduling>> treatmentSchedulingsPerTreatmentId)
        {
            var treatmentSchedulingEntities = treatmentSchedulingsPerTreatmentId
                .SelectMany(_ => _.Value.Select(__ => __.ToScenarioEntity(_.Key)))
                .ToList();

            _unitOfWork.Context.AddAll(treatmentSchedulingEntities);
        }
    }
}
