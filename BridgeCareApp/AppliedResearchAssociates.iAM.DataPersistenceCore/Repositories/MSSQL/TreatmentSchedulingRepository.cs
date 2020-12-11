using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentSchedulingRepository : MSSQLRepository, ITreatmentSchedulingRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public TreatmentSchedulingRepository(IAMContext context) : base(context) { }

        public void CreateTreatmentSchedulings(Dictionary<Guid, List<TreatmentScheduling>> treatmentSchedulingsPerTreatmentId)
        {
            var treatmentSchedulingEntities = treatmentSchedulingsPerTreatmentId
                .SelectMany(_ => _.Value.Select(__ => __.ToEntity(_.Key)))
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.TreatmentScheduling.AddRange(treatmentSchedulingEntities);
            }
            else
            {
                Context.BulkInsert(treatmentSchedulingEntities);
            }
        }
    }
}
