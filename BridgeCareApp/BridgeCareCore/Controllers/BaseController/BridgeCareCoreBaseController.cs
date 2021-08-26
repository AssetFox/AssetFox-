using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers.BaseController
{
    public class BridgeCareCoreBaseController : ControllerBase
    {
        protected readonly IEsecSecurity EsecSecurity;

        protected readonly UnitOfDataPersistenceWork UnitOfWork;

        protected readonly IHubService HubService;

        protected readonly IHttpContextAccessor ContextAccessor;

        private readonly IReadOnlyCollection<string> PathsToIgnore = new List<string>
        {
            "UserTokens", "RevokeToken", "RefreshToken"
        };

        public BridgeCareCoreBaseController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor contextAccessor)
        {
            EsecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            HubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            ContextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            if (RequestHasBearer())
            {
                SetUserInfo(ContextAccessor?.HttpContext?.Request);
            }
        }

        private bool RequestHasBearer()
        {
            if (ContextAccessor?.HttpContext?.Request != null)
            {
                return !PathsToIgnore.Any(pathToIgnore =>
                    ContextAccessor.HttpContext.Request.Path.Value.Contains(pathToIgnore));
            }

            return false;
        }

        public void SetUserInfo(HttpRequest request) => UserInfo = EsecSecurity.GetUserInformation(request);

        private UserInfo _userInfo;
        protected UserInfo UserInfo
        {
            get => _userInfo ?? new UserInfo();
            private set
            {
                if (_userInfo != value)
                {
                    _userInfo = value;
                    CheckIfUserExist();
                }
            }
        }

        private void CheckIfUserExist()
        {
            if (!string.IsNullOrEmpty(UserInfo.Name))
            {
                if (!UnitOfWork.Context.User.Any(_ => _.Username == UserInfo.Name))
                {
                    UnitOfWork.AddUser(UserInfo.Name, UserInfo.Role);
                }

                UnitOfWork.SetUser(_userInfo.Name);
            }
        }

        private Guid UserId => UnitOfWork.UserEntity?.Id ?? Guid.Empty;

        public void CheckUserSimulationReadAuthorization(Guid simulationId)
        {
            if (!UnitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for given scenario.");
            }

            if (UnitOfWork.UserEntity == null || !UnitOfWork.Context.Simulation.Any(_ =>
                _.Id == simulationId && _.SimulationUserJoins.Any(__ => __.UserId == UserId)))
            {
                throw new UnauthorizedAccessException("You are not authorized to view this simulation's data.");
            }
        }

        public void CheckUserSimulationModifyAuthorization(Guid simulationId)
        {
            if (!UnitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found for given scenario.");
            }

            if (!UnitOfWork.Context.Simulation.Any(_ =>
                _.Id == simulationId && _.SimulationUserJoins.Any(__ => __.UserId == UserId && __.CanModify)))
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this simulation's data.");
            }
        }
    }
}
