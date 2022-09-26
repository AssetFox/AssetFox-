﻿using System;
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
using BridgeCareCore.Services;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculatedAttributesController : BridgeCareCoreBaseController
    {
        private readonly ICalculatedAttributesRepository calculatedAttributesRepo;
        private readonly CalculatedAttributeService _calulatedAttributeService;
        private readonly IAttributeRepository attributeRepo;

        public CalculatedAttributesController(IEsecSecurity esec, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, CalculatedAttributeService calulatedAttributeService) : base(esec, unitOfWork, hubService, httpContextAccessor)
        {
            attributeRepo = unitOfWork.AttributeRepo;
            calculatedAttributesRepo = unitOfWork.CalculatedAttributeRepo;
            _calulatedAttributeService = calulatedAttributeService;
        }

        [HttpGet]
        [Route("CalculatedAttributes")]
        [Authorize]
        public async Task<IActionResult> GetCalculatedAttributes()
        {
            var result = await attributeRepo.CalculatedAttributes();
            return Ok(result);
        }
            

        [HttpGet]
        [Route("CalculatedAttrbiuteLibraries")]
        [Authorize]
        public async Task<IActionResult> GetCalculatedAttributeLibraries() =>
             Ok(calculatedAttributesRepo.GetCalculatedAttributeLibraries().ToList());

        [HttpGet]
        [Route("ScenarioAttributes/{simulationId}")]
        [Authorize]
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertLibrary")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryUpsertPagingRequestModel upsertRequest)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var attributes = new List<CalculatedAttributeDTO>();
                if (upsertRequest.SyncModel.LibraryId != null)
                    attributes = _calulatedAttributeService.GetSyncedLibraryDataset(upsertRequest.SyncModel.LibraryId.Value, upsertRequest.SyncModel);
                else if (!upsertRequest.IsNewLibrary)
                    attributes = _calulatedAttributeService.GetSyncedLibraryDataset(upsertRequest.Library.Id, upsertRequest.SyncModel);
                if (upsertRequest.SyncModel.LibraryId != null && upsertRequest.SyncModel.LibraryId != upsertRequest.Library.Id)
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
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Calculated Attribute error::{e.Message}");
                throw;
            }
            return Ok();
        }

        [HttpPost]
        [Route("UpsertScenarioAttribute/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Calculated Attribute error::{e.Message}");
                throw;
            }
            return Ok();
        }

        [HttpPost]
        [Route("UpsertScenarioAttributes/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpsertScenarioAttribute(Guid simulationId, CalculatedAttributePagingSyncModel syncModel)
        {
            if (!SimulationExists(simulationId)) return BadRequest($"Unable to find {simulationId} when upserting simulation attributes");
            try
            {
                UnitOfWork.BeginTransaction();
                var dto = _calulatedAttributeService.GetSyncedScenarioDataset(simulationId, syncModel);
                calculatedAttributesRepo.UpsertScenarioCalculatedAttributes(dto, simulationId);
                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Calculated Attribute error::{e.Message}");
                throw;
            }
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteLibrary(Guid libraryId)
        {
            if (!LibraryIdList().ContainsKey(libraryId)) return BadRequest($"Unable to find {libraryId} in the database");
            try
            {
                calculatedAttributesRepo.DeleteCalculatedAttributeLibrary(libraryId);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Calculated Attribute error::{e.Message}");
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
