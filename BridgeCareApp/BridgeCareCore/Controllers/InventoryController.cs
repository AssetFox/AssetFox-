using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : HubControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IEsecSecurity _esecSecurity;
        private readonly IMaintainableAssetRepository _maintainableAssetRepository;

        public InventoryController(UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IEsecSecurity esecSecurity, IMaintainableAssetRepository maintainableAssetRepository) : base(hubService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _esecSecurity = esecSecurity;
            _maintainableAssetRepository = maintainableAssetRepository;
        }

        [HttpGet]
        [Route("GetInventory")]
        [Authorize]
        public async Task<IActionResult> GetInventory()
        {
            //var userInfo = _esecSecurity.GetUserInformation(Request);

            //var result = await Task.Factory.StartNew(() =>
            //{
            //    _unitOfWork.BeginTransaction();
            //    var userCriteria =
            //        _unitOfWork.UserCriteriaRepo.GetOwnUserCriteria(userInfo.ToDto(),
            //            SecurityConstants.Role.BAMSAdmin);
            //    _unitOfWork.Commit();
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

            return Ok(data);
        }
    }
}
