using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class ValidationController : ApiController
    {
        private readonly IValidationRepository repo;
        private readonly BridgeCareContext db;

        public ValidationController(IValidationRepository repo, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for validating an equation
        /// </summary>
        /// <param name="validationParametersModel">ValidateEquationModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/ValidateEquation")]
        [ModelValidation("The equation data is invalid.")]
        [RestrictAccess]
        public IHttpActionResult ValidateEquation(EquationValidationParametersModel validationParametersModel) =>
            Ok(repo.ValidateEquation(validationParametersModel, db));

        /// <summary>
        /// API endpoint for validating a criteria
        /// </summary>
        /// <param name="model">ValidateCriteriaModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/ValidateCriteria")]
        [ModelValidation("The criteria data is invalid.")]
        [RestrictAccess]
        public IHttpActionResult ValidateCriteria([FromBody]ValidationParameterModel model) =>
            Ok(repo.ValidateCriteria(model.Expression, db));
    }
}
