using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace BridgeCareCore.Controllers
{
    using CommittedProjectRetrieveMethod = Func<Guid, List<SectionCommittedProjectDTO>>;
    using CommittedProjectGetMethod = Func<Guid, FileInfoDTO>;
    using CommittedProjectImportMethod = Action<Guid, ExcelPackage, string, bool>;
    using CommittedProjectUpsertMethod = Action<List<SectionCommittedProjectDTO>>;
    using CommittedProjectDeleteSingleMethod = Action<Guid>;
    using CommittedProjectDeleteMultipleMethod = Action<List<Guid>>;

    [Route("api/[controller]")]
    [ApiController]
    public class CommittedProjectController : BridgeCareCoreBaseController
    {
        private static ICommittedProjectService _committedProjectService;

        private readonly IReadOnlyDictionary<string, CommittedProjectRetrieveMethod> _committedProjectRetrieveMethods;
        private readonly IReadOnlyDictionary<string, CommittedProjectGetMethod> _committedProjectExportMethods;
        private readonly IReadOnlyDictionary<string, CommittedProjectImportMethod> _committedProjectImportMethods;
        private readonly IReadOnlyDictionary<string, CommittedProjectUpsertMethod> _committedProjectUpsertMethods;
        private readonly IReadOnlyDictionary<string, CommittedProjectDeleteSingleMethod> _committedProjectDeleteSimulationProjectMethods;
        private readonly IReadOnlyDictionary<string, CommittedProjectDeleteMultipleMethod> _committedProjectDeleteSpecificMethods;

        public CommittedProjectController(ICommittedProjectService committedProjectService,
            IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor) : base(
            esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _committedProjectService = committedProjectService ??
                                       throw new ArgumentNullException(nameof(committedProjectService));
            _committedProjectRetrieveMethods = CreateRetrieveMethods();
            _committedProjectExportMethods = CreateExportMethods();
            _committedProjectImportMethods = CreateImportMethods();
            _committedProjectUpsertMethods = CreateUpsertMethods();
            _committedProjectDeleteSimulationProjectMethods = CreateDeleteSimulationProjectMethods();
            _committedProjectDeleteSpecificMethods = CreateDeleteSpecificProjectMethods();
        }

        private Dictionary<string, CommittedProjectRetrieveMethod> CreateRetrieveMethods()
        {
            List<SectionCommittedProjectDTO> GetAny(Guid simulationId) => UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);

            List<SectionCommittedProjectDTO> GetPermitted(Guid simulationId)
            {
                CheckUserSimulationReadAuthorization(simulationId);
                return UnitOfWork.CommittedProjectRepo.GetSectionCommittedProjectDTOs(simulationId);
            }

            return new Dictionary<string, CommittedProjectRetrieveMethod>
            {
                [Role.Administrator] = GetAny,
                [Role.DistrictEngineer] = GetPermitted,
                [Role.Cwopa] = GetPermitted,
                [Role.PlanningPartner] = GetPermitted
            };
        }

        private Dictionary<string, CommittedProjectGetMethod> CreateExportMethods()
        {
            FileInfoDTO GetAny(Guid simulationId)
            {
                return _committedProjectService.ExportCommittedProjectsFile(simulationId);
            }

            FileInfoDTO GetPermitted(Guid simulationId)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                return _committedProjectService.ExportCommittedProjectsFile(simulationId);
            }

            return new Dictionary<string, CommittedProjectGetMethod>
            {
                [Role.Administrator] = GetAny,
                [Role.DistrictEngineer] = GetPermitted,
                [Role.Cwopa] = GetPermitted,
                [Role.PlanningPartner] = GetPermitted
            };
        }

        private Dictionary<string, CommittedProjectImportMethod> CreateImportMethods()
        {
            void CreateAny(Guid simulationId, ExcelPackage excelPackage, string filename, bool applyNoTreatment)
            {
                _committedProjectService.ImportCommittedProjectFiles(simulationId, excelPackage, filename, applyNoTreatment);
            }

            void CreatePermitted(Guid simulationId, ExcelPackage excelPackage, string filename, bool applyNoTreatment)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                _committedProjectService.ImportCommittedProjectFiles(simulationId, excelPackage, filename, applyNoTreatment);
            }

            return new Dictionary<string, CommittedProjectImportMethod>
            {
                [Role.Administrator] = CreateAny,
                [Role.DistrictEngineer] = CreatePermitted,
                [Role.Cwopa] = CreatePermitted,
                [Role.PlanningPartner] = CreatePermitted
            };
        }

        private Dictionary<string, CommittedProjectUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(List<SectionCommittedProjectDTO> projects)
            {
                UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(projects);
            }

            void UpsertPermitted(List<SectionCommittedProjectDTO> projects)
            {
                var simulationIds = projects
                    .Where(_ => _.SimulationId != null)
                    .Select(_ => _.SimulationId)
                    .Distinct();
                foreach (var simulation in simulationIds)
                {
                    CheckUserSimulationModifyAuthorization(simulation);
                }

                UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(projects);
            }

            return new Dictionary<string, CommittedProjectUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted,
                [Role.Cwopa] = UpsertPermitted,
                [Role.PlanningPartner] = UpsertPermitted
            };
        }

        private Dictionary<string, CommittedProjectDeleteSingleMethod> CreateDeleteSimulationProjectMethods()
        {
            void DeleteAny(Guid simulationId)
            {
                UnitOfWork.CommittedProjectRepo.DeleteSimulationCommittedProjects(simulationId);
            }

            void DeletePermitted(Guid simulationId)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                DeleteAny(simulationId);
            }

            return new Dictionary<string, CommittedProjectDeleteSingleMethod>
            {
                [Role.Administrator] = DeleteAny,
                [Role.DistrictEngineer] = DeletePermitted,
                [Role.Cwopa] = DeletePermitted,
                [Role.PlanningPartner] = DeletePermitted
            };
        }

        private Dictionary<string, CommittedProjectDeleteMultipleMethod> CreateDeleteSpecificProjectMethods()
        {
            void DeleteAny(List<Guid> simulationIds)
            {
                UnitOfWork.CommittedProjectRepo.DeleteSpecificCommittedProjects(simulationIds);
            }

            void DeletePermitted(List<Guid> simulationIds)
            {
                foreach (var simulation in simulationIds)
                {
                    CheckUserSimulationModifyAuthorization(simulation);
                }
                DeleteAny(simulationIds);
            }

            return new Dictionary<string, CommittedProjectDeleteMultipleMethod>
            {
                [Role.Administrator] = DeleteAny,
                [Role.DistrictEngineer] = DeletePermitted,
                [Role.Cwopa] = DeletePermitted,
                [Role.PlanningPartner] = DeletePermitted
            };
        }

        [HttpPost]
        [Route("ImportCommittedProjects")]
        [Authorize]
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
                    _committedProjectImportMethods[UserInfo.Role](simulationId, excelPackage, filename, applyNoTreatment));

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
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
                    _committedProjectExportMethods[UserInfo.Role](simulationId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
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
        [Authorize]
        public async Task<IActionResult> DeleteSimulationCommittedProjects(Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                    _committedProjectDeleteSimulationProjectMethods[UserInfo.Role](simulationId));

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
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

        [HttpDelete]
        [Route("DeleteSpecificCommittedProjects")]
        [Authorize]
        public async Task<IActionResult> DeleteSpecificCommittedProjects(List<Guid> projectIds)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                    _committedProjectDeleteSpecificMethods[UserInfo.Role](projectIds));
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
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
        [Authorize]
        public async Task<IActionResult> GetCommittedProjects(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                    _committedProjectRetrieveMethods[UserInfo.Role](simulationId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
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
        [Route("UpsertSectionCommittedProjects")]
        [Authorize]
        public async Task<IActionResult> UpsertCommittedProjects(List<SectionCommittedProjectDTO> projects)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                    _committedProjectUpsertMethods[UserInfo.Role](projects));

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
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
    }
}
