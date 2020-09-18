using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AppliedResearchAssociates.iAM.DataPersistence.Models;
using AppliedResearchAssociates.iAM.DataPersistence.Repositories;
using AppliedResearchAssociates.iAM.Segmentation;
using BridgeCare.Domain;
using BridgeCare.Models;
using BridgeCare.Models.AggregationObjects;

namespace BridgeCare.Controllers
{
    public class AggregationController : ApiController
    {
        private readonly Aggregation aggregate;

        public AggregationController(Aggregation agg)
        {
            aggregate = agg ?? throw new ArgumentNullException(nameof(agg));
        }

        /// <summary>
        /// API endpoint for running a Aggregation
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>IHttpActionResult task</returns>
        [HttpPost]
        [Route("api/RunAggregation")]
        public async Task<IHttpActionResult> Post([FromBody] Network network)
        {
            var result = await Task.Factory.StartNew(() => aggregate.Run(network));

            if (!result.IsCompleted)
                return InternalServerError(new Exception(result.Result));

            return Ok();
        }
    }
}
