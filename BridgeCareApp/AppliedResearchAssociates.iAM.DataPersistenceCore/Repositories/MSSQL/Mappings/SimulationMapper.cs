using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class SimulationMapper
    {
        public static SimulationEntity ToEntity(this Simulation domain, Guid networkId) =>
            new SimulationEntity
            {
                Id = Guid.NewGuid(),
                NetworkId = networkId,
                Name = domain.Name,
                NumberOfYearsOfTreatmentOutlook = domain.NumberOfYearsOfTreatmentOutlook
            };

        public static Simulation ToDomain(this SimulationEntity entity) =>
            new Simulation(entity.Network.ToDomain().ToSimulationAnalysisNetworkDomain())
            {
                Name = entity.Name,
                NumberOfYearsOfTreatmentOutlook = entity.NumberOfYearsOfTreatmentOutlook
            };
    }
}
