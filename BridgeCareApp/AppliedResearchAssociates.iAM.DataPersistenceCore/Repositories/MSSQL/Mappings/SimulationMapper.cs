using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

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

        public static void CreateSimulation(this SimulationEntity entity, Network network)
        {
            var simulation = network.AddSimulation();
            simulation.Name = entity.Name;
            simulation.NumberOfYearsOfTreatmentOutlook = entity.NumberOfYearsOfTreatmentOutlook;
        }

        /*public static Simulation ToSimulationAnalysisDomain(this SimulationEntity entity,
            AnalysisMethod analysisMethod,
            List<CommittedProject> committedProjects,
            InvestmentPlan investmentPlan,
            List<PerformanceCurve> performanceCurves,
            List<SelectableTreatment> selectableTreatments)
            {
            var simulation = new Simulation(entity.Network.CreateNetwork())
            {
                Name = entity.Name,
                NumberOfYearsOfTreatmentOutlook = entity.NumberOfYearsOfTreatmentOutlook,
            };

            if (entity.SimulationOutput != null)
            {
                var simulationOutputObject = JsonConvert.DeserializeObject<SimulationOutput>(entity.SimulationOutput.Output, new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                });

                simulation.ClearResults();

                simulation.Results.InitialConditionOfNetwork = simulationOutputObject.InitialConditionOfNetwork;
                simulation.Results.InitialSectionSummaries.AddRange(simulationOutputObject.InitialSectionSummaries);
                simulation.Results.Years.AddRange(simulationOutputObject.Years);
            }

            if (entity.CommittedProjects.Any())
            {
                
            }

            return simulation;
        }*/
    }
}
