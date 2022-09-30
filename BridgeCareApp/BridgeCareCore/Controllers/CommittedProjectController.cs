using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using BridgeCareCore.Utils.Interfaces;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommittedProjectController : BridgeCareCoreBaseController
    {
        private static ICommittedProjectService _committedProjectService;
        private readonly IClaimHelper _claimHelper;
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public CommittedProjectController(ICommittedProjectService committedProjectService,
            IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _committedProjectService = committedProjectService ??
                                       throw new ArgumentNullException(nameof(committedProjectService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
        }

        [HttpPost]
        [Route("ImportCommittedProjects")]
        [Authorize(Policy = Policy.ImportCommittedProjects)]
        public async Task<IActionResult> ImportCommittedProjects()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Committed project file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("simulationId", out var id))
                {
                    throw new ConstraintException("Request contained no simulation id.");
                }

                var simulationId = Guid.Parse(id.ToString());
                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());
                var filename = ContextAccessor.HttpContext.Request.Form.Files[0].FileName;

                var applyNoTreatment = false;
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("applyNoTreatment"))
                {
                    applyNoTreatment = ContextAccessor.HttpContext.Request.Form["applyNoTreatment"].ToString() == "1";
                }

                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    _committedProjectService.ImportCommittedProjectFiles(simulationId, excelPackage, filename, applyNoTreatment);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest($"Committed Project error::{e.Message}");
            }
        }

        [HttpGet]
        [Route("ExportCommittedProjects/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ExportCommittedProjects(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    return _committedProjectService.ExportCommittedProjectsFile(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ValidateAssetExistence/{brkeyValue}")]
        [Authorize]
        public async Task<IActionResult> ValidateAssetExistence(NetworkDTO network, string brkeyValue)
        {
            try
            {
                var isValid = false;
                await Task.Factory.StartNew(() =>
                {
                    isValid = UnitOfWork.MaintainableAssetRepo.CheckIfKeyAttributeValueExists(network.Id, brkeyValue);
                });
                return Ok(isValid);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ValidateExistenceOfAssets/{networkId}")]
        [Authorize]
        public async Task<IActionResult> ValidateExistenceOfAssets(Guid networkId, List<string> brkeys)
        {
            try
            {
                var result = new Dictionary<string, bool>();
                await Task.Factory.StartNew(() =>
                {
                    result = UnitOfWork.MaintainableAssetRepo.CheckIfKeyAttributeValuesExists(networkId, brkeys);
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetTreatmetCost/{brkey}")]
        [Authorize]
        public async Task<IActionResult> GetTreatmetCost(SectionCommittedProjectDTO dto, string brkey)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _committedProjectService.GetTreatmentCost(dto.SimulationId, brkey, dto.Treatment, dto.Year));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetValidConsequences/{brkey}")]
        [Authorize]
        public async Task<IActionResult> GetValidConsequences(SectionCommittedProjectDTO dto, string brkey)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    return _committedProjectService.GetValidConsequences(dto.Id, dto.SimulationId, brkey, dto.Treatment, dto.Year);
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("FillTreatmentValues/{brkey}")]
        [Authorize]
        public async Task<IActionResult> FillTreatmentValues(SectionCommittedProjectDTO dto, string brkey)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    dto.Consequences =  _committedProjectService.GetValidConsequences(dto.Id, dto.SimulationId, brkey, dto.Treatment, dto.Year);
                    dto.Cost = _committedProjectService.GetTreatmentCost(dto.SimulationId, brkey, dto.Treatment, dto.Year);
                    var treatment = UnitOfWork.Context.ScenarioSelectableTreatment
                        .FirstOrDefault(_ => _.Name == dto.Treatment && _.SimulationId == dto.SimulationId);
                    if (treatment == null)
                        return dto;
                    dto.Category = (TreatmentCategory)treatment.Category;
                    return dto;
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("CommittedProjectTemplate")]
        [Authorize]
        public async Task<IActionResult> GetCommittedProjectTemplate()
        {
            var result = await Task.Factory.StartNew(() => _committedProjectService.CreateCommittedProjectTemplate());
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteSimulationCommittedProjects/{simulationId}")]
        [Authorize(Policy = Policy.ModifyCommittedProjects)]
        public async Task<IActionResult> DeleteSimulationCommittedProjects(Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.CommittedProjectRepo.DeleteSimulationCommittedProjects(simulationId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                return Ok();
            }
            catch (RowNotInTableException)
            {
                return BadRequest($"Unable to find simulation {simulationId}");
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("DeleteSpecificCommittedProjects")]
        [Authorize(Policy = Policy.ModifyCommittedProjects)]
        public async Task<IActionResult> DeleteSpecificCommittedProjects(List<Guid> projectIds)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    CheckDeletePermit(projectIds);
                    UnitOfWork.CommittedProjectRepo.DeleteSpecificCommittedProjects(projectIds);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                return Ok();
            }
            catch (RowNotInTableException)
            {
                return BadRequest($"Unable to find the simulations referenced by the provided committed projects");
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }            
        }

        [HttpGet]
        [Route("GetSectionCommittedProjects/{simulationId}")]
        [Authorize(Policy = Policy.ViewCommittedProjects)]
        public async Task<IActionResult> GetCommittedProjects(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    return UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                return Ok();
            }
            catch (RowNotInTableException)
            {
                return BadRequest($"Unable to find simulation {simulationId}");
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [Route("GetSectionCommittedProjectsPage/{simulationId}")]
        [Authorize]
        // TODO New method add claims and policy here
        public async Task<IActionResult> GetCommittedProjectsPage(Guid simulationId, PagingRequestModel<SectionCommittedProjectDTO> request)
        {
            try
            {
                var result = new PagingPageModel<SectionCommittedProjectDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    var sectionCommittedProjectDTOs = UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
                    result = _committedProjectService.GetCommittedProjectPage(sectionCommittedProjectDTOs, request);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                return Ok();
            }
            catch (RowNotInTableException)
            {
                return BadRequest($"Unable to find simulation {simulationId}");
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertSectionCommittedProjects/{simulationId}")]
        [Authorize(Policy = Policy.ModifyCommittedProjects)]
        public async Task<IActionResult> UpsertCommittedProjects(Guid simulationId, PagingSyncModel<SectionCommittedProjectDTO> request)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var projects = _committedProjectService.GetSyncedDataset(simulationId, request);
                    CheckUpsertPermit(projects);
                    UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(projects);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                return Ok();
            }
            catch (RowNotInTableException)
            {
                return BadRequest($"Unable to find simulations matching the project description");
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }            
        }

        private void CheckDeletePermit(List<Guid> projectIds)
        {
            if (_claimHelper.RequirePermittedCheck())
            {
                foreach (var project in projectIds)
                {
                    try
                    {
                        var simulationId = UnitOfWork.CommittedProjectRepo.GetSimulationId(project);
                        _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    }
                    catch (RowNotInTableException)
                    {
                        // Do nothing - project not found
                    }
                }
            }
        }

        void CheckUpsertPermit(List<SectionCommittedProjectDTO> projects)
        {
            if (_claimHelper.RequirePermittedCheck())
            {
                var simulationIds = projects
                .Select(_ => _.SimulationId)
                .Distinct();
                foreach (var simulation in simulationIds)
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulation, UserId);
                }
            }
        }
    }
}
