using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public static class ISimulationRepositoryExtensions
    {
        /// <summary>Gets the name if posible. If that throws, instead returns the id with a message. All exceptions are caught,
        /// so external code will never see this throw.</summary> 
        public static string GetSimulationNameOrId(this ISimulationRepository simulationRepository, Guid simulationId)
        {
            string simulationName;
            try
            {
                simulationName = simulationRepository.GetSimulationName(simulationId);
            }
            catch
            {
                simulationName = $"simulationId.ToString(); failed to get name";
            }
            return simulationName;
        }

        /// <summary>Gets the name if posible. If that throws, instead returns the id with a message. All exceptions are caught,
        /// so external code will never see this throw.</summary> 
        public static string GetSimulationNameOrId(this ISimulationRepository repository, string simulationIdAsString)
        {
            if (Guid.TryParse(simulationIdAsString, out Guid simulationId))
            {
                return GetSimulationNameOrId(repository, simulationId);
            }
            return $"{simulationIdAsString ?? "null"}; failed to parse to guid.";
        }
    }
}
