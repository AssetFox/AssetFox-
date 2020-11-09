using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AnalysisMethodRepository : MSSQLRepository, IAnalysisMethodRepository
    {
        public AnalysisMethodRepository(IAMContext context) : base(context) { }

        public void CreateAnalysisMethod(AnalysisMethod domain, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            if (!Context.Attribute.Any(_ => _.Name == domain.Weighting.Name))
            {
                throw new RowNotInTableException($"No attribute found having name {domain.Weighting.Name}");
            }

            var simulation = Context.Simulation.Single(_ => _.Name == simulationName);
            var attribute = Context.Attribute.Single(_ => _.Name == domain.Weighting.Name);

            Context.AnalysisMethod.Add(domain.ToEntity(simulation.Id, attribute.Id));
            Context.SaveChanges();
        }

        public AnalysisMethod GetSimulationAnalysisMethod(string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            return Context.AnalysisMethod.Include(_ => _.Simulation)
                .ThenInclude(_ => _.Network)
                .Include(_ => _.Attribute)
                .Include(_ => _.Benefit)
                .Include(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Single(_ => _.Simulation.Name == simulationName)
                .ToDomain();
        }
    }
}
