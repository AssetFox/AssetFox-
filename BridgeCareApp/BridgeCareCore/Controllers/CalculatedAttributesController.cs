using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BridgeCareCore.Models;
using BridgeCareCore.Interfaces;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using Microsoft.SqlServer.Dac.Model;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCore.Utils;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculatedAttributesController : BridgeCareCoreBaseController
    {
        public const string CalculatedAttributeError = "Calculated Attribute Error";
        private readonly ICalculatedAttributesRepository calculatedAttributesRepo;
        private readonly ICalculatedAttributePagingService _calulatedAttributeService;
        private readonly IAttributeRepository attributeRepo;
        private readonly IClaimHelper _claimHelper;

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public CalculatedAttributesController(IEsecSecurity esec, IUnitOfWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper, ICalculatedAttributePagingService calulatedAttributeService) : base(esec, unitOfWork, hubService, httpContextAccessor)
        {
            attributeRepo = unitOfWork.AttributeRepo;
            calculatedAttributesRepo = unitOfWork.CalculatedAttributeRepo;
            _calulatedAttributeService = calulatedAttributeService;
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
        }

        [HttpGet]
        [Route("CalculatedAttributes")]
        [ClaimAuthorize("CalculatedAttributesViewAccess")]
        public async Task<IActionResult> GetCalculatedAttributes()
        {
            var result = await attributeRepo.CalculatedAttributes();
            return Ok(result);
        }

        [HttpGet]
        [Route("GetEmptyCalculatedAttributesByLibraryId/{libraryId}")]
        [ClaimAuthorize("CalculatedAttributesViewAccess")]
        public async Task<IActionResult> GetEmptyCalculatedAttributesByLibraryId(Guid libraryId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.CalculatedAttributeRepo.GetCalcuatedAttributesByLibraryIdNoChildren(libraryId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError,
                    $"{CalculatedAttributeError}::{nameof(GetEmptyCalculatedAttributesByLibraryId)} - {HubService.errorList["Exception"]}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetEmptyCalculatedAttributesByScenarioId/{scenarioId}")]
        [ClaimAuthorize("CalculatedAttributesViewAccess")]
        public async Task<IActionResult> GetEmptyCalculatedAttributesByScenarioId(Guid scenarioId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.CalculatedAttributeRepo.GetCalcuatedAttributesByScenarioIdNoChildren(scenarioId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError,
                    $"{CalculatedAttributeError}::{nameof(GetEmptyCalculatedAttributesByScenarioId)} - {HubService.errorList["Exception"]}");
                throw;
            }
        }


        [HttpGet]
        [Route("CalculatedAttrbiuteLibraries")]
        [ClaimAuthorize("CalculatedAttributesViewAccess")]
        public async Task<IActionResult> GetCalculatedAttributeLibraries() =>
             Ok(calculatedAttributesRepo.GetCalculatedAttributeLibraries().ToList());

        [HttpGet]
        [Route("ScenarioAttributes/{simulationId}")]
        [ClaimAuthorize("CalculatedAttributesViewAccess")]
        public async Task<IActionResult> GetAttributesForScenario(Guid simulationId)
        {
            if (!SimulationExists(simulationId)) return BadRequest($"Unable to find {simulationId} when getting simulation attributes");
            return Ok(calculatedAttributesRepo.GetScenarioCalculatedAttributes(simulationId));
        }

        [HttpPost]
        [Route("GetScenarioCalculatedAttrbiutetPage/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioCalculatedAttrbiutetPage(Guid simulationId, CalculatedAttributePagingRequestModel pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _calulatedAttributeService.GetLibraryPage(simulationId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::GetScenarioCalculatedAttributePage - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetLibraryCalculatedAttrbiutePage/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> GetLibraryCalculatedAttrbiutePage(Guid libraryId, CalculatedAttributePagingRequestModel pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _calulatedAttributeService.GetScenarioPage(libraryId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::GetLibraryCalculatedAttributePage - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertLibrary")]
        [Authorize(Policy = Policy.ModifyCalculatedAttributesFromLibrary)]
        public async Task<IActionResult> UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryUpsertPagingRequestModel upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var attributes = new List<CalculatedAttributeDTO>();
                    if (upsertRequest.ScenarioId != null)
                        attributes = _calulatedAttributeService.GetSyncedScenarioDataSet(upsertRequest.ScenarioId.Value, upsertRequest.SyncModel);
                    else if (upsertRequest.SyncModel.LibraryId != null)
                        attributes = _calulatedAttributeService.GetSyncedLibraryDataset(upsertRequest.SyncModel.LibraryId.Value, upsertRequest.SyncModel);
                    else if (!upsertRequest.IsNewLibrary)
                        attributes = _calulatedAttributeService.GetSyncedLibraryDataset(upsertRequest.Library.Id, upsertRequest.SyncModel);
                    if (upsertRequest.IsNewLibrary)
                        attributes.ForEach(attribute =>
                        {
                            attribute.Id = Guid.NewGuid();
                            var equations = attribute.Equations.ToList();
                            equations.ForEach(_ =>
                            {
                                _.Id = Guid.NewGuid();
                                _.Equation.Id = Guid.NewGuid();
                                _.CriteriaLibrary.Id = Guid.NewGuid();
                            });
                            attribute.Equations = equations;
                        });
                    var dto = upsertRequest.Library;
                    dto.CalculatedAttributes = attributes;
                    calculatedAttributesRepo.UpsertCalculatedAttributeLibrary(dto);
                });
                return Ok();

            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::UpsertCalculatedAttributeLibrary - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioAttribute/{simulationId}")]
        [Authorize(Policy = Policy.ModifyCalculatedAttributesFromScenario)]
        public async Task<IActionResult> UpsertScenarioAttribute(Guid simulationId, CalculatedAttributeDTO dto)
        {
            if (!SimulationExists(simulationId)) return BadRequest($"Unable to find {simulationId} when upserting a simulation attribute");
            var dtoList = new List<CalculatedAttributeDTO>() { dto };
            try
            {
                calculatedAttributesRepo.UpsertScenarioCalculatedAttributes(dtoList, simulationId);
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::UpsertScenarioAttribute for {simulationName} - {e.Message}");
                throw;
            }
            return Ok();
        }

        [HttpPost]
        [Route("UpsertScenarioAttributes/{simulationId}")]
        [Authorize(Policy = Policy.ModifyCalculatedAttributesFromScenario)]
        public async Task<IActionResult> UpsertScenarioAttribute(Guid simulationId, CalculatedAttributePagingSyncModel syncModel)
        {
            if (!SimulationExists(simulationId)) return BadRequest($"Unable to find {simulationId} when upserting simulation attributes");
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var dto = _calulatedAttributeService.GetSyncedScenarioDataSet(simulationId, syncModel);
                    UnitOfWork.BeginTransaction();

                    calculatedAttributesRepo.AddLibraryIdToScenarioCalculatedAttributes(dto, syncModel.LibraryId);
                    calculatedAttributesRepo.AddModifiedToScenarioCalculatedAttributes(dto, syncModel.IsModified);
                    
                    calculatedAttributesRepo.UpsertScenarioCalculatedAttributes(dto, simulationId);
                });
                return Ok();
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::UpsertScenarioAttributes for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteLibrary/{libraryId}")]
        [Authorize(Policy = Policy.ModifyCalculatedAttributesFromLibrary)]
        public async Task<IActionResult> DeleteLibrary(Guid libraryId)
        {
            if (!LibraryIdList().ContainsKey(libraryId)) return BadRequest($"Unable to find {libraryId} in the database");
            try
            {
                calculatedAttributesRepo.DeleteCalculatedAttributeLibrary(libraryId);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::DeleteLibrary - {e.Message}");
                throw;
            } 
            return Ok();
        }
        [HttpGet]
        [Route("GetIsSharedLibrary/{calculatedAttributeLimitLibraryId}")]
        [ClaimAuthorize("CalculatedAttributesViewAccess")]
        public async Task<IActionResult> GetIsSharedLibrary(Guid calculatedAttributeLimitLibraryId)
        {
            bool result = false;
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var users = UnitOfWork.CalculatedAttributeRepo.GetLibraryUsers(calculatedAttributeLimitLibraryId);
                    if (users.Count <= 0)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::GetIsSharedLibrary - {e.Message}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetCalculatedAttributeLibraryUsers/{libraryId}")]
        [ClaimAuthorize("CalculatedAttributesViewAccess")]
        public async Task<IActionResult> GetCalculatedAttributeLibraryUsers(Guid libraryId)
        {
            try
            {
                List<LibraryUserDTO> users = new List<LibraryUserDTO>();
                await Task.Factory.StartNew(() =>
                {
                    var accessModel = UnitOfWork.CalculatedAttributeRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.RequirePermittedCheck();
                    users = UnitOfWork.CalculatedAttributeRepo.GetLibraryUsers(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::GetCalculatedAttributeLibraryUsers - {e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::GetCalculatedAttributeLibraryUsers - {e.Message}");
                throw;
            }
        }
        [HttpPost]
        [Route("UpsertOrDeleteCalculatedAttributeLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ModifyCalculatedAttributesFromLibrary)]
        public async Task<IActionResult> UpsertOrDeleteCalculatedAttributeLibraryUsers(Guid libraryId, List<LibraryUserDTO> proposedUsers)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryUsers = UnitOfWork.CalculatedAttributeRepo.GetLibraryUsers(libraryId);
                    _claimHelper.RequirePermittedCheck();
                    UnitOfWork.CalculatedAttributeRepo.UpsertOrDeleteUsers(libraryId, proposedUsers);
                });
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::UpsertOrDeleteCalculatedAttributeLibraryUsers - {e.Message}");
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::UpsertOrDeleteCalculatedAttributeLibraryUsers - {e.Message}");
                return BadRequest();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CalculatedAttributeError}::UpsertOrDeleteCalculatedAttributeLibraryUsers - {e.Message}");
                throw;
            }
        }
        // Helpers
        private Dictionary<Guid, string> LibraryIdList() =>
            calculatedAttributesRepo.GetCalculatedAttributeLibraries().Select(_ => new { _.Name, _.Id }).ToDictionary(_ => _.Id, _ => _.Name);

        private bool SimulationExists(Guid simulationId) =>
            (UnitOfWork.SimulationRepo.GetSimulationName(simulationId) == null) ? false : true;
    }
}
