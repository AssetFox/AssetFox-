using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : BridgeCareCoreBaseController
    {
        private readonly IMaintainableAssetRepository _maintainableAssetRepository;

        public InventoryController(IMaintainableAssetRepository maintainableAssetRepository, IEsecSecurity esecSecurity,
            UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor) :
            base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _maintainableAssetRepository = maintainableAssetRepository;

        [HttpGet]
        [Route("GetInventory")]
        [Authorize]
        public async Task<IActionResult> GetInventory()
        {
            //var userInfo = _esecSecurity.GetUserInformation(Request);

            //var result = await Task.Factory.StartNew(() =>
            //{
            //    UnitOfWork.BeginTransaction();
            //    var userCriteria =
            //        UnitOfWork.UserCriteriaRepo.GetOwnUserCriteria(userInfo.ToDto(),
            //            SecurityConstants.Role.BAMSAdmin);
            //    UnitOfWork.Commit();
            //    return userCriteria;
            //});


            //if (!result.HasAccess)
            //{
            //    throw new UnauthorizedAccessException($"User {result.UserName} has no inventory access.");
            //}
            //if (result.HasCriteria)
            //{
            //    result.Criteria = "(" + result.Criteria + ")";

            //   // expression += $" AND { result.Criteria }";
            //}
            var data = _maintainableAssetRepository.GetBMSIDAndBRKey();
            data.Sort((a, b) => a.BrKey.CompareTo(b.BrKey));
            return Ok(data);
        }
    }
}
