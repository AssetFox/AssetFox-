using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class SegmentationController : ApiController
    {
        private readonly Segmentation Segmentation;

        public SegmentationController(Segmentation segmentation)
        {
            Segmentation = segmentation ?? throw new ArgumentNullException(nameof(segmentation));
        }

        /// <summary>
        /// API endpoint for running a Segmentation
        /// </summary>
        /// <param name="model">Network</param>
        /// <returns>IHttpActionResult task</returns>
        [HttpPost]
        [Route("api/ResegmentNetwork")]
        public async Task<IHttpActionResult> Post([FromBody] string networkId)
        {
            var result = await Task.Factory.StartNew(() => Segmentation.Run(networkId));

            if (!result.IsCompleted)
                return InternalServerError(new Exception(result.Result));

            return Ok();
        }

        [HttpPost]
        [Route("api/CreateSegmentation")]
        public async Task<IHttpActionResult> Post()
        {
            var result = await Task.Factory.StartNew(() => Segmentation.CreateSegmentation());

            if (!result.IsCompleted)
                return InternalServerError(new Exception(result.Result));

            return Ok();
        }
    }
}
