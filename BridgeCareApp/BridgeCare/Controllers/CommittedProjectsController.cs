using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    using CommittedProjectsDeleteMethod = Action<int, BridgeCareContext, UserInformationModel>;

    public class CommittedProjectsController : ApiController
    {
        private readonly ICommittedProjects _committedProjectRepo;
        private readonly ICommittedRepository _committedRepo;
        private readonly BridgeCareContext _db;
        private readonly IReadOnlyDictionary<string, CommittedProjectsDeleteMethod> _committedProjectsDeleteMethods;

        public CommittedProjectsController(ICommittedProjects committedProjectRepo, ICommittedRepository committedRepo, BridgeCareContext db)
        {
            _committedProjectRepo = committedProjectRepo ?? throw new ArgumentNullException(nameof(committedProjectRepo));
            _committedRepo = committedRepo ?? throw new ArgumentNullException(nameof(committedRepo));
            _db = db ?? throw new ArgumentNullException(nameof(db));

            _committedProjectsDeleteMethods = CreateDeleteMethods();
        }

        private Dictionary<string, CommittedProjectsDeleteMethod> CreateDeleteMethods()
        {
            void DeleteAnyCommittedProjects(int simulationId, BridgeCareContext db, UserInformationModel userInformation) =>
                _committedRepo.DeleteCommittedProjects(simulationId, db);
            void DeletePermittedCommittedProjects(int simulationId, BridgeCareContext db, UserInformationModel userInformation) =>
                _committedRepo.DeletePermittedCommittedProjects(simulationId, db, userInformation.Name);

            return new Dictionary<string, CommittedProjectsDeleteMethod>
            {
                [Role.ADMINISTRATOR] = DeleteAnyCommittedProjects,
                [Role.DISTRICT_ENGINEER] = DeletePermittedCommittedProjects,
                [Role.CWOPA] = DeletePermittedCommittedProjects,
                [Role.PLANNING_PARTNER] = DeletePermittedCommittedProjects
            };
        }

        /// <summary>
        /// API endpoint for saving committed projects data
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SaveCommittedProjectsFiles")]
        [RestrictAccess]
        public IHttpActionResult SaveCommittedProjectsFiles()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new ConstraintException("The data provided is not a valid MIME type.");

            UserInformationModel userInformation = ESECSecurity.GetUserInformation(Request);
            _committedProjectRepo.SaveCommittedProjectsFiles(HttpContext.Current.Request, _db, userInformation);
            return Ok();
        }

        /// <summary>
        /// API endpoint for creating committed projects excel file
        /// </summary>
        /// <param name="model">SimulationModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/ExportCommittedProjects")]
        [ModelValidation("The scenario data is invalid.")]
        [RestrictAccess]
        public HttpResponseMessage ExportCommittedProjects([FromBody]SimulationModel model)
        {
            var response = Request.CreateResponse();
            var userInformation = ESECSecurity.GetUserInformation(Request);
            byte[] byteArray = _committedProjectRepo.ExportCommittedProjects(model.simulationId, model.networkId, _db, userInformation);
            response.Content = new ByteArrayContent(byteArray);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "CommittedProjects.xlsx"
            };

            return response;
            //return Ok(response);
        }

        /// <summary>
        /// API endpoint for deleting committed projects
        /// </summary>
        /// <param name="simulationId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/DeleteCommittedProjects/{simulationId}")]
        [RestrictAccess]
        public IHttpActionResult DeleteCommittedProjects(int simulationId)
        {
            var userInformation = ESECSecurity.GetUserInformation(Request);
            _committedProjectsDeleteMethods[userInformation.Role](simulationId, _db, userInformation);
            return Ok();
        }
    }
}
