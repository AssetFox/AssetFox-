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
    using CommittedProjectGetMethod = Func<Guid, FileInfoDTO>;
    using CommittedProjectCreateMethod = Action<Guid, ExcelPackage, string, bool>;
    using CommittedProjectDeleteMethod = Action<Guid>;

    [Route("api/[controller]")]
    [ApiController]
    public class CommittedProjectController : BridgeCareCoreBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private static ICommittedProjectService _committedProjectService;

        private readonly IReadOnlyDictionary<string, CommittedProjectGetMethod> _committedProjectExportMethods;
        private readonly IReadOnlyDictionary<string, CommittedProjectCreateMethod> _committedProjectImportMethods;
        private readonly IReadOnlyDictionary<string, CommittedProjectDeleteMethod> _committedProjectDeleteMethods;

        public CommittedProjectController(ICommittedProjectService committedProjectService,
            IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor) : base(
            esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _committedProjectService = committedProjectService ??
                                       throw new ArgumentNullException(nameof(committedProjectService));
            _committedProjectExportMethods = CreateExportMethods();
            _committedProjectImportMethods = CreateImportMethods();
            _committedProjectDeleteMethods = CreateDeleteMethods();
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

        private Dictionary<string, CommittedProjectCreateMethod> CreateImportMethods()
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

            return new Dictionary<string, CommittedProjectCreateMethod>
            {
                [Role.Administrator] = CreateAny,
                [Role.DistrictEngineer] = CreatePermitted,
                [Role.Cwopa] = CreatePermitted,
                [Role.PlanningPartner] = CreatePermitted
            };
        }

        private Dictionary<string, CommittedProjectDeleteMethod> CreateDeleteMethods()
        {
            void DeleteAny(Guid simulationId)
            {
                UnitOfWork.CommittedProjectRepo.DeleteCommittedProjects(simulationId);
            }

            void DeletePermitted(Guid simulationId)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                DeleteAny(simulationId);
            }

            return new Dictionary<string, CommittedProjectDeleteMethod>
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
                {
                    UnitOfWork.BeginTransaction();
                    _committedProjectImportMethods[UserInfo.Role](simulationId, excelPackage, filename, applyNoTreatment);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
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
        [Route("DeleteCommittedProjects/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCommittedProjects(Guid simulationId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _committedProjectDeleteMethods[UserInfo.Role](simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch (RowNotInTableException)
            {
                UnitOfWork.Rollback();
                return BadRequest($"Unable to find simulation {simulationId}");
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Committed Project error::{e.Message}");
                throw;
            }
        }
    }
}
