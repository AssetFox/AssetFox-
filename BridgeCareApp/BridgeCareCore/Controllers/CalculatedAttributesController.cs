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

        public CalculatedAttributesController(IEsecSecurity esec, IUnitOfWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, ICalculatedAttributePagingService calulatedAttributeService) : base(esec, unitOfWork, hubService, httpContextAccessor)
        {
            attributeRepo = unitOfWork.AttributeRepo;
            calculatedAttributesRepo = unitOfWork.CalculatedAttributeRepo;
            _calulatedAttributeService = calulatedAttributeService;
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
                var result = await Task.Factory.StartNew(() => _calulatedAttributeService.GetScenarioCalculatedAttributePage(simulationId, pageRequest));
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
                var result = await Task.Factory.StartNew(() => _calulatedAttributeService.GetLibraryCalculatedAttributePage(libraryId, pageRequest));
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
                    UnitOfWork.BeginTransaction();
                    var attributes = new List<CalculatedAttributeDTO>();
                    if (upsertRequest.ScenarioId != null)
                        attributes = _calulatedAttributeService.GetSyncedScenarioDataset(upsertRequest.ScenarioId.Value, upsertRequest.SyncModel);
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
                    UnitOfWork.Commit();
                });
                return Ok();

            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
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
                    var dto = _calulatedAttributeService.GetSyncedScenarioDataset(simulationId, syncModel);
                    UnitOfWork.BeginTransaction();
                    
                    calculatedAttributesRepo.UpsertScenarioCalculatedAttributes(dto, simulationId);
                    UnitOfWork.Commit();
                });
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
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

        // Helpers
        private Dictionary<Guid, string> LibraryIdList() =>
            calculatedAttributesRepo.GetCalculatedAttributeLibraries().Select(_ => new { _.Name, _.Id }).ToDictionary(_ => _.Id, _ => _.Name);

        private bool SimulationExists(Guid simulationId) =>
            (UnitOfWork.SimulationRepo.GetSimulationName(simulationId) == null) ? false : true;
    }
}
