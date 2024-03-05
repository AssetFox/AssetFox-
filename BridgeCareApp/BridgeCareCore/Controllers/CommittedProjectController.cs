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
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using BridgeCareCore.Services;
using BridgeCareCore.Services.General_Work_Queue.WorkItems;
using System.IO;
using Org.BouncyCastle.Utilities;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommittedProjectController : BridgeCareCoreBaseController
    {
        public const string CommittedProjectError = "Committed Project Error";
        private static ICommittedProjectService _committedProjectService;
        private static ICommittedProjectPagingService _committedProjectPagingService;
        private readonly IClaimHelper _claimHelper;
        private readonly IGeneralWorkQueueService _generalWorkQueueService;
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public CommittedProjectController(ICommittedProjectService committedProjectService, ICommittedProjectPagingService committedProjectPagingService,
            IEsecSecurity esecSecurity,
            IUnitOfWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor httpContextAccessor,
            IClaimHelper claimHelper, IGeneralWorkQueueService generalWorkQueueService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _committedProjectService = committedProjectService ?? throw new ArgumentNullException(nameof(committedProjectService));
            _committedProjectPagingService = committedProjectPagingService ?? throw new ArgumentNullException(nameof(committedProjectPagingService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _generalWorkQueueService = generalWorkQueueService ?? throw new ArgumentNullException(nameof(generalWorkQueueService));
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

                var siulationName = "";
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    siulationName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                });
                ImportCommittedProjectWorkItem workItem = new ImportCommittedProjectWorkItem(simulationId, excelPackage, filename, UserInfo.Name, siulationName);
                var analysisHandle = _generalWorkQueueService.CreateAndRunInFastQueue(workItem);

                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastFastWorkQueueUpdate, simulationId.ToString());

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::ImportCommittedProjects - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::ImportCommittedProjects - {e.Message}");
                throw;
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
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::ExportCommittedProjects for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::ExportCommittedProjects for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("DownloadCommittedProjectTemplate")]
        [Authorize]
        public async Task<IActionResult> DownloadCommittedProjectTemplate()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.CommittedProjectRepo.DownloadCommittedProjectTemplate());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"::Unable to DownloadedTemplate - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ValidateAssetExistence/{keyAttrValue}")]
        [Authorize]
        public async Task<IActionResult> ValidateAssetExistence(NetworkDTO network, string keyAttrValue)
        {
            try
            {
                var isValid = false;
                await Task.Factory.StartNew(() =>
                {
                    isValid = UnitOfWork.MaintainableAssetRepo.CheckIfKeyAttributeValueExists(network.Id, keyAttrValue);
                });
                return Ok(isValid);
            }
            catch (Exception e)
            {
                var networkName = network.Name ?? "null";
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::ValidateAssetExistence for {networkName} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ValidateExistenceOfAssets/{networkId}")]
        [Authorize]
        public async Task<IActionResult> ValidateExistenceOfAssets(Guid networkId, List<string> keyattrValues)
        {
            try
            {
                var result = new Dictionary<string, bool>();
                await Task.Factory.StartNew(() =>
                {
                    result = UnitOfWork.MaintainableAssetRepo.CheckIfKeyAttributeValuesExists(networkId, keyattrValues);
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                var networkName = UnitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::ValidateExistenceOfAssets for {networkName} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("FillTreatmentValues")]
        [Authorize]
        public async Task<IActionResult> FillTreatmentValues(CommittedProjectFillTreatmentValuesModel treatmentValues)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                {
                    CommittedProjectFillTreatmentReturnValuesModel returnValues = new CommittedProjectFillTreatmentReturnValuesModel();
                    var treatment = UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatmentByLibraryIdAndName(
                        treatmentValues.TreatmentLibraryId, treatmentValues.TreatmentName);
                    if (treatment == null)
                        return returnValues;
                    returnValues.ValidTreatmentConsequences =  _committedProjectService.GetValidConsequences(treatmentValues.CommittedProjectId, treatmentValues.TreatmentLibraryId,
                        treatmentValues.KeyAttributeValue, treatmentValues.TreatmentName, treatmentValues.NetworkId);
                    returnValues.TreatmentCost = _committedProjectService.GetTreatmentCost(treatmentValues.TreatmentLibraryId,
                        treatmentValues.KeyAttributeValue, treatmentValues.TreatmentName, treatmentValues.NetworkId);
                    
                    returnValues.TreatmentCategory = (TreatmentCategory)treatment.Category;
                    return returnValues;
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::FillTreatmentValues - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("CommittedProjectTemplate/{networkId}")]
        [Authorize]
        public async Task<IActionResult> GetCommittedProjectTemplate(Guid networkId)
        {
            var result = await Task.Factory.StartNew(() => _committedProjectService.CreateCommittedProjectTemplate(networkId));
            return Ok(result);
        }

        [HttpPost]
        [Route("SetCommittedProjectTemplate")]
        [Authorize(Policy = Policy.ModifyCommittedProjects)]
        public async Task<IActionResult> SetCommittedProjectTemplate()
        {
            Stream stream = ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.CommittedProjectRepo.SetCommittedProjectTemplate(stream);
                });
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastTaskCompleted, "Successfully Updated Implementation Name");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"::Unable to Upload Template - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("getUploadedCommittedProjectTemplates")]
        [Authorize]
        public async Task<IActionResult> getUploadedCommittedProjectTemplates()
        {
            var result = await Task.Factory.StartNew(() => UnitOfWork.CommittedProjectRepo.getUploadedCommittedProjectTemplates());
            return Ok(result);
        }

        [HttpPost]
        [Route("AddCommittedProjectTemplate")]
        [Authorize(Policy = Policy.ModifyCommittedProjects)]
        public async Task<IActionResult> AddCommittedProjectTemplate()
        {
            Stream stream = ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream();
            String filename = ContextAccessor.HttpContext.Request.Form.Files[0].FileName;
            int fileExtPos = filename.LastIndexOf(".");
            if (fileExtPos >= 0)
                filename = filename.Substring(0, fileExtPos);

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.CommittedProjectRepo.AddCommittedProjectTemplate(stream, filename);
                });
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastTaskCompleted, "Successfully Updated Implementation Name");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"::Unable to Upload Template - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("DownloadSelectedCommittedProjectTemplate/{filename}")]
        [Authorize]
        public async Task<IActionResult> DownloadSelectedCommittedProjectTemplate(string filename)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.CommittedProjectRepo.DownloadSelectedCommittedProjectTemplate(filename));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"::Unable to DownloadedTemplate - {e.Message}");
                throw;
            }
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
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::DeleteSimulationCommittedProjects for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (RowNotInTableException)
            {
                return BadRequest($"Unable to find simulation {simulationId}");
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::DeleteSimulationCommittedProjects for {simulationName} - {e.Message}");
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
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::DeleteSpecificCommittedProjects - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (RowNotInTableException)
            {
                return BadRequest($"Unable to find the simulations referenced by the provided committed projects");
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::DeleteSpecificCommittedProjects - {e.Message}");
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
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::GetCommittedProjects - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (RowNotInTableException)
            {
                return BadRequest($"Unable to find simulation {simulationId}");
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::GetCommittedProjects for {simulationName} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetSectionCommittedProjectsPage/{simulationId}")]
        [Authorize(Policy = Policy.ViewCommittedProjects)]
        public async Task<IActionResult> GetCommittedProjectsPage(Guid simulationId, PagingRequestModel<SectionCommittedProjectDTO> request)
        {
            try
            {
                var result = new PagingPageModel<SectionCommittedProjectDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    var sectionCommittedProjectDTOs = UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
                    result = _committedProjectPagingService.GetCommittedProjectPage(sectionCommittedProjectDTOs, request);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::GetCommittedProjectsPage - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (RowNotInTableException)
            {
                return BadRequest($"Unable to find simulation {simulationId}");
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::GetCommittedProjectsPage for {simulationName} - {e.Message}");
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
                    var projects = _committedProjectPagingService.GetSyncedDataset(simulationId, request);
                    CheckUpsertPermit(projects);
                    UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(projects);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::UpsertCommittedProjects - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (RowNotInTableException)
            {
                return BadRequest($"Unable to find simulations matching the project description");
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{CommittedProjectError}::UpsertCommittedProjects for {simulationName} - {e.Message}");
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

        [HttpGet("projectsources")]
        public IActionResult GetProjectSources()
        {
            var projectSources = Enum.GetNames(typeof(ProjectSourceDTO)).ToList();
            return Ok(projectSources);
        }

        private void CheckUpsertPermit(List<SectionCommittedProjectDTO> projects)
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
