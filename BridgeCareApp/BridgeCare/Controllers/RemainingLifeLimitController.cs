﻿using System;
using System.Web.Http;
using System.Web.Http.Filters;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;

namespace BridgeCare.Controllers
{
    public class RemainingLifeLimitController : ApiController
    {
        private readonly IRemainingLifeLimitRepository repo;
        private readonly BridgeCareContext db;

        public RemainingLifeLimitController(IRemainingLifeLimitRepository repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching a simulation's remaining life limit library data
        /// </summary>
        /// <param name="id">Simulation identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetScenarioRemainingLifeLimitLibrary/{id}")]
        [ModelValidation("The scenario id is invalid.")]
        [RestrictAccess(Role.ADMINISTRATOR)]
        public IHttpActionResult GetSimulationRemainingLifeLimitLibrary(int id) =>
            Ok(repo.GetSimulationRemainingLifeLimitLibrary(id, db));

        /// <summary>
        /// API endpoint for upserting/deleting a simulation's remaining life limit library data
        /// </summary>
        /// <param name="model">RemainingLifeLimitLibraryModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveScenarioRemainingLifeLimitLibrary")]
        [RestrictAccess(Role.ADMINISTRATOR)]
        public IHttpActionResult SaveSimulationRemainingLifeLimitLibrary([FromBody]RemainingLifeLimitLibraryModel model) =>
            Ok(repo.SaveSimulationRemainingLifeLimitLibrary(model, db));
    }
}
