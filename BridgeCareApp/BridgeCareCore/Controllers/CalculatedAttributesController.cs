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
        [Route("ScenarioAttributes")]
        //[Authorize]
        public async Task<IActionResult> GetAttributesForScenario(Guid simulationId) => Ok(calculatedAttributesRepo.GetScenarioCalculatedAttributes(simulationId));

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
            var dtoList = new List<CalculatedAttributeDTO>() { dto };
            calculatedAttributesRepo.UpsertScenarioCalculatedAttributes(dtoList, simulationId);
            return Ok();
        }

        [HttpPost]
        [Route("UpsertScenarioAttributes")]
        //[Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpsertScenarioAttribute(Guid simulationId, List<CalculatedAttributeDTO> dto)
        {
            calculatedAttributesRepo.UpsertScenarioCalculatedAttributes(dto, simulationId);
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteLibrary")]
        //[Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteLibrary(Guid libraryId)
        {
            calculatedAttributesRepo.DeleteCalculatedAttributeLibrary(libraryId);
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteScenarioAttribute")]
        //[Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteAttributeFromScenario(Guid simulationId, Guid attributeId)
        {
            calculatedAttributesRepo.DeleteCalculatedAttributeFromScenario(simulationId, attributeId);
            return Ok();
        }
    }
}
