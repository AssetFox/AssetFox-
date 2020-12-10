using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using SQLitePCL;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class PerformanceCurveRepository : MSSQLRepository, IPerformanceCurveRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly IEquationRepository _equationRepo;
        private readonly ICriterionLibraryRepository _criterionLibraryRepo;

        public PerformanceCurveRepository(IEquationRepository equationRepo, ICriterionLibraryRepository criterionLibraryRepo, IAMContext context) : base(context)
        {
            _equationRepo = equationRepo ?? throw new ArgumentNullException(nameof(equationRepo));
            _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
        }

        public void CreatePerformanceCurveLibrary(string name, Guid simulationId)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Id == simulationId))
                    {
                        throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                    }

                    var performanceCurveLibraryEntity = new PerformanceCurveLibraryEntity {Id = Guid.NewGuid(), Name = name};

                    Context.PerformanceCurveLibrary.Add(performanceCurveLibraryEntity);

                    Context.PerformanceCurveLibrarySimulation.Add(new PerformanceCurveLibrarySimulationEntity
                    {
                        PerformanceCurveLibraryId = performanceCurveLibraryEntity.Id, SimulationId = simulationId
                    });

                    Context.SaveChanges();

                    contextTransaction.Commit();
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public void CreatePerformanceCurves(List<PerformanceCurve> performanceCurves, Guid simulationId)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Id == simulationId))
                    {
                        throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                    }

                    var simulationEntity = Context.Simulation
                        .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                        .Single(_ => _.Id == simulationId);

                    if (simulationEntity.PerformanceCurveLibrarySimulationJoin == null)
                    {
                        throw new RowNotInTableException($"No performance curve library found for simulation having id {simulationId}.");
                    }

                    var attributeNames = performanceCurves.Select(_ => _.Attribute.Name).Distinct().ToList();
                    var attributeEntities = Context.Attribute
                        .Where(_ => attributeNames.Contains(_.Name)).ToList();

                    if (!attributeEntities.Any())
                    {
                        throw new RowNotInTableException("Could not find matching attributes for given performance curves.");
                    }

                    var attributeNamesFromDataSource = attributeEntities.Select(_ => _.Name).ToList();
                    if (!attributeNames.All(ruleName => attributeNamesFromDataSource.Contains(ruleName)))
                    {
                        var attributeNamesNotFound = attributeNames.Except(attributeNamesFromDataSource).ToList();
                        if (attributeNamesNotFound.Count() == 1)
                        {
                            throw new RowNotInTableException($"No attribute found having name {attributeNamesNotFound[0]}.");
                        }

                        throw new RowNotInTableException(
                            $"No attributes found found having names: {string.Join(", ", attributeNamesNotFound)}.");
                    }

                    var performanceCurveEntities = performanceCurves
                        .Select(_ => _.ToEntity(simulationEntity.PerformanceCurveLibrarySimulationJoin.PerformanceCurveLibraryId,
                            attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id))
                        .ToList();

                    if (IsRunningFromXUnit)
                    {
                        Context.PerformanceCurve.AddRange(performanceCurveEntities);
                    }
                    else
                    {
                        Context.BulkInsert(performanceCurveEntities);
                    }

                    Context.SaveChanges();

                    if (performanceCurves.Any(_ => !_.Equation.ExpressionIsBlank))
                    {
                        var equationEntityPerPerformanceCurveEntityId = performanceCurves
                            .Where(_ => !_.Equation.ExpressionIsBlank)
                            .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                        _equationRepo.CreateEquations(equationEntityPerPerformanceCurveEntityId, "PerformanceCurveEntity");
                    }

                    if (performanceCurves.Any(_ => !_.Criterion.ExpressionIsBlank))
                    {
                        var curveIdsPerExpression = performanceCurves
                            .Where(_ => !_.Criterion.ExpressionIsBlank)
                            .GroupBy(_ => _.Criterion.Expression, _ => _)
                            .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                        _criterionLibraryRepo.JoinEntitiesWithCriteria(curveIdsPerExpression, "PerformanceCurveEntity", simulationEntity.Name);
                    }

                    contextTransaction.Commit();
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public void GetSimulationPerformanceCurves(Simulation simulation)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having name {simulation.Name}");
            }

            Context.PerformanceCurve
                .Include(_ => _.Attribute)
                .Include(_ => _.CriterionLibraryPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.PerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.PerformanceCurveLibrary)
                .ThenInclude(_ => _.PerformanceCurveLibrarySimulationJoins)
                .Where(_ => _.PerformanceCurveLibrary.PerformanceCurveLibrarySimulationJoins
                    .SingleOrDefault(__ => __.Simulation.Name == simulation.Name) != null)
                .ForEach(_ => _.CreatePerformanceCurve(simulation));
        }
    }
}
