using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using OfficeOpenXml;
using MoreLinq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using System.Data;
using Newtonsoft.Json;

namespace BridgeCareCore.Services
{
    public class PerformanceCurvesPagingService : IPerformanceCurvesPagingService
    {
        private static IUnitOfWork _unitOfWork;        

        public PerformanceCurvesPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public PagingPageModel<PerformanceCurveDTO> GetLibraryPerformanceCurvePage(Guid libraryId, PagingRequestModel<PerformanceCurveDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<PerformanceCurveDTO>();
            var curves = _unitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibraryOrderedById(libraryId);

            curves = SyncedDataset(curves, request.SyncModel);

            if (request.search.Trim() != "")
                curves = SearchCurves(curves, request.search);
            if (request.sortColumn.Trim() != "")
                curves = OrderByColumn(curves, request.sortColumn, request.isDescending);

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = curves.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = curves;
                return new PagingPageModel<PerformanceCurveDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<PerformanceCurveDTO>()
            {
                Items = items,
                TotalItems = curves.Count()
            };
        }

        public PagingPageModel<PerformanceCurveDTO> GetScenarioPerformanceCurvePage(Guid simulationId, PagingRequestModel<PerformanceCurveDTO> request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<PerformanceCurveDTO>();
            var curves = request.SyncModel.LibraryId == null ? _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurvesOrderedById(simulationId) :
                _unitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibraryOrderedById(request.SyncModel.LibraryId.Value);

            curves = SyncedDataset(curves, request.SyncModel);

            if (request.search != null && request.search.Trim() != "")
                curves = SearchCurves(curves, request.search);
            if (request.sortColumn != null && request.sortColumn.Trim() != "")
                curves = OrderByColumn(curves, request.sortColumn, request.isDescending);

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = curves.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = curves;
                return new PagingPageModel<PerformanceCurveDTO>()
                {
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new PagingPageModel<PerformanceCurveDTO>()
            {
                Items = items,
                TotalItems = curves.Count()
            };
        }

        public List<PerformanceCurveDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<PerformanceCurveDTO> request)
        {
            var curves = new List<PerformanceCurveDTO>();
            if (request.LibraryId == null)
            {
                curves = _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            }
            else
            {
                curves = _unitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibrary(request.LibraryId.Value);
                // Create new performance curves based on provided library
                foreach (var curve in curves)
                {
                    curve.Id = Guid.NewGuid();
                }
            }

            return SyncedDataset(curves, request);
        }

        public List<PerformanceCurveDTO> GetSyncedLibraryDataset(Guid libraryId, PagingSyncModel<PerformanceCurveDTO> request)
        {
            var curves = _unitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibrary(libraryId);
            return SyncedDataset(curves, request);
        }

        public List<PerformanceCurveDTO> GetNewLibraryDataset(PagingSyncModel<PerformanceCurveDTO> request)
        {
            var curves = new List<PerformanceCurveDTO>();
            return SyncedDataset(curves, request);
        }

        private List<PerformanceCurveDTO> OrderByColumn(List<PerformanceCurveDTO> curves, string sortColumn, bool isDescending)
        {
            sortColumn = sortColumn?.ToLower();
            switch (sortColumn)
            {
            case "name":
                if (isDescending)
                    return curves.OrderByDescending(_ => _.Name.ToLower()).ToList();
                else
                    return curves.OrderBy(_ => _.Name.ToLower()).ToList();
            case "attribute":
                if (isDescending)
                    return curves.OrderByDescending(_ => _.Attribute.ToLower()).ToList();
                else
                    return curves.OrderBy(_ => _.Attribute.ToLower()).ToList();
            }
            return curves;
        }

        private List<PerformanceCurveDTO> SearchCurves(List<PerformanceCurveDTO> curves, string search)
        {
            var lowerCaseSearch = search.ToLower();
            return curves
                .Where(_ => _.Name.ToLower().Contains(lowerCaseSearch) ||
                    _.Attribute.ToLower().Contains(lowerCaseSearch) ||
                    (_.Equation.Expression != null && _.Equation.Expression.ToLower().Contains(lowerCaseSearch)) ||
                    (_.CriterionLibrary.MergedCriteriaExpression != null && _.CriterionLibrary.MergedCriteriaExpression.ToLower().Contains(lowerCaseSearch))).ToList();
        }

        private List<PerformanceCurveDTO> SyncedDataset(List<PerformanceCurveDTO> curves, PagingSyncModel<PerformanceCurveDTO> request)
        {
            curves = curves.Concat(request.AddedRows).Where(_ => !request.RowsForDeletion.Contains(_.Id)).ToList();

            for (var i = 0; i < curves.Count; i++)
            {
                var item = request.UpdateRows.FirstOrDefault(row => row.Id == curves[i].Id);
                if (item != null)
                    curves[i] = item;
            }

            return curves;
        }
    }
}
