using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationOutputRepository : ISimulationOutputRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public SimulationOutputRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateSimulationOutput(Guid simulationId, SimulationOutput simulationOutput)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking()
                .Single(_ => _.Id == simulationId);

            if (simulationOutput == null)
            {
                throw new InvalidOperationException($"No results found for simulation {simulationEntity.Name}. Please ensure that the simulation analysis has been run.");
            }


            try
            {
                _unitOfWork.Context.DeleteAll<SimulationOutputEntity>(_ =>
                _.SimulationId == simulationId);
                var entity = SimulationOutputMapper.ToEntity(simulationOutput, simulationId);
                _unitOfWork.Context.Add(entity);
                _unitOfWork.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void GetSimulationOutput(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if (_unitOfWork.Context.SimulationOutput.Any(_ => _.SimulationId == simulation.Id))
            {
                _unitOfWork.Context.SimulationOutput.Single(_ => _.SimulationId == simulation.Id)
                    .FillSimulationResults(simulation);
            }
        }

        public SimulationOutput GetSimulationOutput(Guid simulationId)
        {
            throw new NotImplementedException();
            //if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            //{
            //    throw new RowNotInTableException($"Found no simulation having id {simulationId}");
            //}

            //if (!_unitOfWork.Context.SimulationOutput.Any(_ => _.SimulationId == simulationId))
            //{
            //    throw new RowNotInTableException($"No simulation analysis results were found for simulation having id {simulationId}. Please ensure that the simulation analysis has been run.");
            //}

            //var simulationOutputObjects = _unitOfWork.Context.SimulationOutput.Where(_ => _.SimulationId == simulationId);

            //var simulationOutput = new SimulationOutput();
            //foreach (var item in simulationOutputObjects)
            //{
            //    switch (item.OutputType)
            //    {
            //    case SimulationOutputEnum.YearlySection:
            //        var yearlySections = JsonConvert.DeserializeObject<SimulationYearDetail>(item.Output, new JsonSerializerSettings
            //        {
            //            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            //        });
            //        simulationOutput.Years.Add(yearlySections);
            //        break;
            //    case SimulationOutputEnum.InitialConditionNetwork:
            //        simulationOutput.InitialConditionOfNetwork = Convert.ToDouble(item.Output);
            //        break;
            //    case SimulationOutputEnum.InitialSummary:
            //        var initialSummary = JsonConvert.DeserializeObject<List<AssetSummaryDetail>>(item.Output, new JsonSerializerSettings
            //        {
            //            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            //        });
            //        simulationOutput.InitialAssetSummaries.AddRange(initialSummary);
            //        break;
            //    }
            //}
            //simulationOutput.Years.Sort((a, b) => a.Year.CompareTo(b.Year));

            ////var outputData = JsonConvert.DeserializeObject<SimulationOutput>(simulationOutputString, new JsonSerializerSettings
            ////{
            ////    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            ////});

            //return simulationOutput;
        }
    }
}
