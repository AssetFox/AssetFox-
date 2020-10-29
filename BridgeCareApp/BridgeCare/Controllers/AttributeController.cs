using BridgeCare.Interfaces;
using BridgeCare.Security;
using System;
using System.Web.Http;
using BridgeCare.Models;

namespace BridgeCare.Controllers
{
    public class AttributeController : ApiController
    {
        private readonly IAttributeRepository _repository;
        private readonly BridgeCareContext db;

        public AttributeController(IAttributeRepository repository, BridgeCareContext db)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching all attributes
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetAttributes")]
        [RestrictAccess]
        public IHttpActionResult GetAttributes() => Ok(_repository.GetAttributes(db));

        [HttpPost]
        [Route("api/GetAttributesSelectValues")]
        [RestrictAccess]
        public IHttpActionResult GetAttributeSelectValues([FromBody] NetworkAttributes model) => Ok(_repository.GetAttributeSelectValues(model, db));
    }
}
