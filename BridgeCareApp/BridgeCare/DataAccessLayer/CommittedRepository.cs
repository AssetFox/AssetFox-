using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using EntityFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Microsoft.SqlServer.Management.Dmf;

namespace BridgeCare.DataAccessLayer
{
    public class CommittedRepository : ICommittedRepository
    {
        /// <summary>
        /// Save committed projects in the database
        /// </summary>
        /// <param name="committedProjectModels"></param>
        /// <param name="db"></param>
        public void SaveCommittedProjects(int simulationId, List<CommittedProjectModel> committedProjectModels, BridgeCareContext db)
        {
            DeleteCommittedProjects(simulationId, db);
            db.CommittedProjects.AddRange(committedProjectModels.Select(model => new CommittedEntity(model)).ToList());
            db.SaveChanges();
        }

        /// <summary>
        /// Runs a parameterized query string to delete the committed projects from the database with the given simulation id
        /// </summary>
        /// <param name="simulationId">Simulation id to use in the query 'where' clause</param>
        /// <param name="db">Database context used to execute the query</param>
        public void DeleteCommittedProjects(int simulationId, BridgeCareContext db)
        {
            var deleteCommittedProjectsQuery = @"DELETE FROM [dbo].[COMMITTED_] WHERE SIMULATIONID = @SimulationId";
            db.Database.ExecuteSqlCommand(deleteCommittedProjectsQuery,
                new SqlParameter("@SimulationId", simulationId));
        }

        public void DeletePermittedCommittedProjects(int simulationId, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                throw new RowNotInTableException($"No simulation found with id {simulationId}");

            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == simulationId).UserCanModify(username))
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario's committed projects.");

            DeleteCommittedProjects(simulationId, db);
        }

        /// <summary>
        /// Save committed projects in the database, if the user owns the scenario
        /// </summary>
        /// <param name="committedProjectModels"></param>
        /// <param name="db"></param>
        public void SavePermittedCommittedProjects(int simulationId, List<CommittedProjectModel> committedProjectModels, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == simulationId))
            {
                if (simulationId == 0)
                {
                    throw new InvalidOperationException("Simulation id was not provided.");
                }
                throw new RowNotInTableException($"No simulation found with id {simulationId}");
            }

            if (!db.Simulations.Include(s => s.USERS).Single(s => s.SIMULATIONID == simulationId).UserCanModify(username))
                throw new UnauthorizedAccessException("You are not authorized to modify this scenario's committed projects.");

            SaveCommittedProjects(simulationId, committedProjectModels, db);
        }

        /// <summary>
        /// Get all the committed projects for a given simulation id
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<CommittedEntity> GetCommittedProjects(int simulationId, BridgeCareContext db)
        {
            return db.CommittedProjects.Include(c => c.COMMIT_CONSEQUENCES).Where(c => c.SIMULATIONID == simulationId).ToList();
        }

        /// <summary>
        /// Get all the committed projects for a given simulation id, if owned by the current user
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<CommittedEntity> GetPermittedCommittedProjects(int simulationId, BridgeCareContext db, string username)
        {
            if (!db.Simulations.Any(s => s.SIMULATIONID == simulationId))
                throw new RowNotInTableException($"No scenario found with id {simulationId}.");
            if (!db.Simulations.Include(s => s.USERS).First(s => s.SIMULATIONID == simulationId).UserCanRead(username))
                throw new UnauthorizedAccessException("You are not authorized to view this scenario's committed projects.");
            return GetCommittedProjects(simulationId, db);
        }
    }
}
