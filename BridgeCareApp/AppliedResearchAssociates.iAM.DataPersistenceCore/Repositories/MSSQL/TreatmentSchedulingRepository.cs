using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentSchedulingRepository : ITreatmentSchedulingRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly IAMContext _context;

        public TreatmentSchedulingRepository(IAMContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public void CreateTreatmentSchedulings(Dictionary<Guid, List<TreatmentScheduling>> treatmentSchedulingsPerTreatmentId)
        {
            var treatmentSchedulingEntities = treatmentSchedulingsPerTreatmentId
                .SelectMany(_ => _.Value.Select(__ => __.ToEntity(_.Key)))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _context.TreatmentScheduling.AddRange(treatmentSchedulingEntities);
            }
            else
            {
                _context.BulkInsert(treatmentSchedulingEntities);
            }
        }
    }
}
