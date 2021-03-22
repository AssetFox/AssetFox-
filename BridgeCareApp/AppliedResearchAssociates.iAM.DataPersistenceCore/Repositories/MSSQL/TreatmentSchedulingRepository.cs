using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentSchedulingRepository : ITreatmentSchedulingRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public TreatmentSchedulingRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateTreatmentSchedulings(Dictionary<Guid, List<TreatmentScheduling>> treatmentSchedulingsPerTreatmentId)
        {
            var treatmentSchedulingEntities = treatmentSchedulingsPerTreatmentId
                .SelectMany(_ => _.Value.Select(__ => __.ToEntity(_.Key)))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.TreatmentScheduling.AddRange(treatmentSchedulingEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(treatmentSchedulingEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
