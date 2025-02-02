﻿using BridgeCare.Interfaces;
using BridgeCare.Security;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using BridgeCare.Models;

namespace BridgeCare.Controllers
{
    public class RunRollupController : ApiController
    {
        private readonly IRunRollupRepository repo;
        private readonly BridgeCareContext db;

        public RunRollupController(IRunRollupRepository repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for running a rollup
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>IHttpActionResult task</returns>
        [HttpPost]
        [Route("api/RunRollup")]
        [RestrictAccess]
        public async Task<IHttpActionResult> Post([FromBody]SimulationModel model)
        {
            var result = await Task.Factory.StartNew(() => repo.RunRollup(model));

            if (!result.IsCompleted)
                return InternalServerError(new Exception(result.Result));

            return Ok();
        }
    }
}
