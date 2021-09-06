using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculatedAttributesController : BridgeCareCoreBaseController
    {
        private readonly ICalculatedAttributesRepository calculatedAttributesRepo;

        public CalculatedAttributesController(IEsecSecurity esec, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esec, unitOfWork, hubService, httpContextAccessor)
        {
            calculatedAttributesRepo = unitOfWork.CalculatedAttributeRepo;
        }

        [HttpGet]
        [Route("CalculatedAttributeList")]
        //[Authorize]
        public async Task<IActionResult> GetAttributeList()
        {
            var calculatedAttributes = await UnitOfWork.AttributeRepo.CalculatedAttributes();
            return Ok(calculatedAttributes);
        }

        [HttpGet]
        [Route("CalculatedAttrbiuteLibraries")]
        [Authorize]
        public async Task<IActionResult> GetCalculatedAttributeLibraries()
        {
            return Ok(LibraryIdList());
        }

        [HttpGet]
        [Route("AttributeLibrary")]
        [Authorize]
        public async Task<IActionResult> GetCalculatedAttributeLibrary(Guid libraryId)
        {
            if (!LibraryIdList().ContainsKey(libraryId)) return BadRequest($"Unable to find {libraryId} in the database");
            var result = calculatedAttributesRepo.GetCalculatedAttributeLibraries().FirstOrDefault(_ => _.Id == libraryId);
            if (result == null) return StatusCode(500);
            return Ok(result);
        }

        [HttpGet]
        [Route("ScenarioAttributes")]
        //[Authorize]
        public async Task<IActionResult> GetAttributesForScenario(Guid simulationId)
        {
            if (!SimulationExists(simulationId)) return BadRequest($"Unable to find {simulationId} when getting simulation attributes");
            return Ok(calculatedAttributesRepo.GetScenarioCalculatedAttributes(simulationId));
        }

        [HttpPost]
        [Route("UpsertLibrary")]
        //[Authorize(Policy = SecurityConstants.Policy.Admin]
        public async Task<IActionResult> UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO dto)
        {
            calculatedAttributesRepo.UpsertCalculatedAttributeLibrary(dto);
            return Ok();
        }

        [HttpPost]
        [Route("UpsertScenarioAttribute")]
        //[Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpsertScenarioAttribute(Guid simulationId, CalculatedAttributeDTO dto)
        {
            if (!SimulationExists(simulationId)) return BadRequest($"Unable to find {simulationId} when upserting a simulation attribute");
            var dtoList = new List<CalculatedAttributeDTO>() { dto };
            calculatedAttributesRepo.UpsertScenarioCalculatedAttributes(dtoList, simulationId);
            return Ok();
        }

        [HttpPost]
        [Route("UpsertScenarioAttributes")]
        //[Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpsertScenarioAttribute(Guid simulationId, List<CalculatedAttributeDTO> dto)
        {
            if (!SimulationExists(simulationId)) return BadRequest($"Unable to find {simulationId} when upserting simulation attributes");
            calculatedAttributesRepo.UpsertScenarioCalculatedAttributes(dto, simulationId);
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteLibrary")]
        //[Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteLibrary(Guid libraryId)
        {
            if (!LibraryIdList().ContainsKey(libraryId)) return BadRequest($"Unable to find {libraryId} in the database");
            calculatedAttributesRepo.DeleteCalculatedAttributeLibrary(libraryId);
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteScenarioAttribute")]
        //[Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteAttributeFromScenario(Guid simulationId, Guid attributeId)
        {
            if (!SimulationExists(simulationId)) return BadRequest($"Unable to find {simulationId} when deleting a simulation attribute");
            calculatedAttributesRepo.DeleteCalculatedAttributeFromScenario(simulationId, attributeId);
            return Ok();
        }

        // Helpers
        private Dictionary<Guid, string> LibraryIdList() =>
            calculatedAttributesRepo.GetCalculatedAttributeLibraries().Select(_ => new { _.Name, _.Id }).ToDictionary(_ => _.Id, _ => _.Name);

        private bool SimulationExists(Guid simulationId) =>
            UnitOfWork.NetworkRepo.GetAllNetworks().Any(_ => _.Id == simulationId);
    }
}
