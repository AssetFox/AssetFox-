using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    public class UserCriteriaController : ControllerBase
    {
        private readonly IUserCriteriaRepository repo;
        private readonly IEsecSecurity _esecSecurity;

        public UserCriteriaController(IEsecSecurity esecSecurity,IUserCriteriaRepository repo)
        {
            _esecSecurity = esecSecurity;
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// API endpoint for fetching the current user's criteria settings.
        /// Uses the authorization token to determine user identity
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetUserCriteria")]
        //[RestrictAccess]
        [Authorize]
        public IActionResult GetUserCriteria()
        {
            var userInformation = _esecSecurity.GetUserInformation(Request);
            var data = new UserInfoDTO
            {
                Email = userInformation.Email,
                Roles = userInformation.Role,
                Sub = userInformation.Name
            };
            var adminCheckConst = SecurityConstants.Policy.BAMSAdmin;
            return Ok(repo.GetOwnUserCriteria(data, adminCheckConst));
        }

        /// <summary>
        /// API endpoint for fetching all users' criteria settings.
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetAllUserCriteria")]
        //[RestrictAccess(Role.ADMINISTRATOR)]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public IActionResult GetAllUserCriteria() => Ok(repo.GetAllUserCriteria());

        /// <summary>
        /// API endpoint for modifying a user's criteria settings
        /// </summary>
        /// <param name="userCriteria">UserCriteriaModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("api/SetUserCriteria")]
        //[RestrictAccess(Role.ADMINISTRATOR)]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public IActionResult SetUserCriteria([FromBody] UserCriteriaDTO userCriteria)
        {
            repo.SaveUserCriteria(userCriteria);
            return Ok();
        }

        /// <summary>
        /// API endpoint for deleting a user's criteria
        /// </summary>
        /// <param name="username">User's username</param>
        /// <returns>IHttpActionResult</returns>
        [HttpDelete]
        [Route("api/RevokeUserAccess/{userCriteriaId}")]
        //[RestrictAccess(Role.ADMINISTRATOR)]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public IActionResult RevokeUserAccess(Guid userCriteriaId)
        {
            repo.RevokeUserAccess(userCriteriaId);
            return Ok();
        }

        [HttpDelete]
        [Route("api/DeleteUser/{userId}")]
        //[RestrictAccess(Role.ADMINISTRATOR)]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public IActionResult DeleteUser(Guid userId)
        {
            repo.DeleteUser(userId);
            return Ok();
        }
    }
}
