﻿using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;

namespace BridgeCare.DataAccessLayer
{
    public class RemainingLifeLimitRepository : IRemainingLifeLimitRepository
    {
        /// <summary>
        /// Fetches a simulation's remaining life limit library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>RemainingLifeLimitLibraryModel</returns>
        public RemainingLifeLimitLibraryModel GetSimulationRemainingLifeLimitLibrary(int id, BridgeCareContext db)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}");
                
            var simulation = db.Simulations.Include(s => s.REMAINING_LIFE_LIMITS).Single(s => s.SIMULATIONID == id);

            return new RemainingLifeLimitLibraryModel(simulation);
        }

        /// <summary>
        /// Executes an upsert/delete operation on a simulation's remaining life limit library data
        /// </summary>
        /// <param name="model">RemainingLifeLimitLibraryModel</param>
        /// <param name="db">BridgeCareContext</param>
        /// <returns>RemainingLifeLimitLibraryModel</returns>
        public RemainingLifeLimitLibraryModel SaveSimulationRemainingLifeLimitLibrary(RemainingLifeLimitLibraryModel model,
            BridgeCareContext db)
        {
            var id = int.Parse(model.Id);

            if (!db.Simulations.Any(s => s.SIMULATIONID == id))
                throw new RowNotInTableException($"No scenario was found with id {id}");

            var simulation = db.Simulations.Include(s => s.REMAINING_LIFE_LIMITS).Single(s => s.SIMULATIONID == id);

            if (simulation.REMAINING_LIFE_LIMITS.Any())
            {
                simulation.REMAINING_LIFE_LIMITS.ToList().ForEach(remainingLifeLimitEntity =>
                {
                    var remainingLifeLimitModel = model.RemainingLifeLimits.SingleOrDefault(m =>
                        m.Id == remainingLifeLimitEntity.REMAINING_LIFE_ID.ToString());

                    if (remainingLifeLimitModel == null)
                        RemainingLifeLimitsEntity.DeleteEntry(remainingLifeLimitEntity, db);
                    else
                    {
                        remainingLifeLimitModel.matched = true;
                        remainingLifeLimitModel.Update(remainingLifeLimitEntity);
                    }
                });
            }

            if (model.RemainingLifeLimits.Any(m => !m.matched))
                db.RemainingLifeLimits
                    .AddRange(model.RemainingLifeLimits
                        .Where(remainingLifeLimitModel => !remainingLifeLimitModel.matched)
                        .Select(remainingLifeLimitModel => new RemainingLifeLimitsEntity(id, remainingLifeLimitModel))
                        .ToList()
                    );

            db.SaveChanges();

            return new RemainingLifeLimitLibraryModel(simulation);
        }
    }
}
