using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class PerformanceCurveRepository : MSSQLRepository, IPerformanceCurveRepository
    {
        public static readonly bool IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

        public PerformanceCurveRepository(IAMContext context) : base(context) { }

        public void CreatePerformanceCurveLibrary(PerformanceCurveLibraryEntity entity,
            string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation.Single(_ => _.Name == simulationName);

            Context.PerformanceCurveLibrary.Add(entity);

            Context.PerformanceCurveLibrarySimulation.Add(new PerformanceCurveLibrarySimulationEntity
            {
                PerformanceCurveLibraryId = entity.Id, SimulationId = simulationEntity.Id
            });

            Context.SaveChanges();
        }

        public int CreatePerformanceCurves(List<PerformanceCurve> domains, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var attributeNames = domains.Select(_ => _.Attribute.Name).Distinct().ToList();

            if (!Context.Attribute.Any(_ => attributeNames.Contains(_.Name)))
            {
                throw new RowNotInTableException("Could not find matching attributes for given performance curves");
            }

            var simulationEntity = Context.Simulation.Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                .Single(_ => _.Name == simulationName);

            var attributeIdPerName = Context.Attribute.Where(_ => attributeNames.Contains(_.Name))
                .ToDictionary(e => e.Name, e => e.Id);

            var entities = domains.Select(_ =>
                _.ToEntity(simulationEntity.PerformanceCurveLibrarySimulationJoin.PerformanceCurveLibraryId,
                    attributeIdPerName[_.Attribute.Name]))
                .ToList();

            if (!entities.Any())
            {
                return 0;
            }

            if (IsRunningFromNUnit)
            {
                Context.PerformanceCurve.AddRange(entities);
            }
            else
            {
                Context.BulkInsert(entities);
            }

            Context.SaveChanges();

            return entities.Count();
        }

        public IEnumerable<PerformanceCurve> GetSimulationPerformanceCurves(string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation.Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                .ThenInclude(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.Attribute)
                .Single(_ => _.Name == simulationName);

            return null;
        }
    }
}
