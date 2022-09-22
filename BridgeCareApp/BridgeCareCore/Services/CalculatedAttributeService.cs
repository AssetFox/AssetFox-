﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using OfficeOpenXml;
using BridgeCareCore.Interfaces.DefaultData;

namespace BridgeCareCore.Services
{
    public class CalculatedAttributeService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public CalculatedAttributeService(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public PagingPageModel<CalculatedAttributeEquationCriteriaPairDTO> GetLibraryCalculatedAttributePage(Guid libraryId, CalculatedAttributePagingRequestModel request)
        {
            var attribute = _unitOfWork.CalculatedAttributeRepo.GetLibraryCalulatedAttributesByLibraryAndAttributeId(libraryId, request.AttributeId);
            return HandlePaging(attribute, request);
        }

        public PagingPageModel<CalculatedAttributeEquationCriteriaPairDTO> GetScenarioCalculatedAttributePage(Guid simulationId, CalculatedAttributePagingRequestModel request)
        {
            var attribute = request.SyncModel.LibraryId == null ? _unitOfWork.CalculatedAttributeRepo.GetScenarioCalulatedAttributesByScenarioAndAttributeId(simulationId, request.AttributeId):
                _unitOfWork.CalculatedAttributeRepo.GetLibraryCalulatedAttributesByLibraryAndAttributeId(request.SyncModel.LibraryId.Value, request.AttributeId);

            return HandlePaging(attribute, request);
        }

        public List<CalculatedAttributeDTO> GetSyncedScenarioDataset(Guid simulationId, CalculatedAttributePagingSyncModel request)
        {
            var attributes = request.LibraryId == null ?
                    _unitOfWork.CalculatedAttributeRepo.GetScenarioCalculatedAttributes(simulationId).ToList() :
                    _unitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraryByID(request.LibraryId.Value).CalculatedAttributes.ToList();
            return SyncedDataset(attributes, request);
        }

        public List<CalculatedAttributeDTO> GetSyncedLibraryDataset(Guid libraryId, CalculatedAttributePagingSyncModel request)
        {
            var library = _unitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraryByID(libraryId);
            return SyncedDataset(library.CalculatedAttributes.ToList() , request);
        }

        private List<CalculatedAttributeEquationCriteriaPairDTO> OrderByColumn(List<CalculatedAttributeEquationCriteriaPairDTO> equations, string sortColumn, bool isDescending)
        {
            sortColumn = sortColumn?.ToLower();
            switch (sortColumn)
            {
            case "equation":
                if (isDescending)
                    return equations.OrderByDescending(_ => _.Equation.Expression.ToLower()).ToList();
                else
                    return equations.OrderBy(_ => _.Equation.Expression.ToLower()).ToList();
            case "criterion":
                if (isDescending)
                    return equations.OrderByDescending(_ => _.CriteriaLibrary.MergedCriteriaExpression.ToLower()).ToList();
                else
                    return equations.OrderBy(_ => _.CriteriaLibrary.MergedCriteriaExpression.ToLower()).ToList();
            }
            return equations;
        }

        private List<CalculatedAttributeEquationCriteriaPairDTO> SearchRows(List<CalculatedAttributeEquationCriteriaPairDTO> equations, string search)
        {
            return equations
                .Where(_ => (_.Equation.Expression != null && _.Equation.Expression.ToLower().Contains(search)) ||
                    (_.CriteriaLibrary.MergedCriteriaExpression != null && _.CriteriaLibrary.MergedCriteriaExpression.ToLower().Contains(search))).ToList();
        }

        private List<CalculatedAttributeDTO> SyncedDataset(List<CalculatedAttributeDTO> attributes, CalculatedAttributePagingSyncModel syncModel)
        {

            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var item = syncModel.UpdatedCalculatedAttributes.FirstOrDefault(row => row.Id == attribute.Id);
                if (item != null)
                {
                    attribute.CalculationTiming = item.CalculationTiming;
                }
                if (syncModel.AddedPairs.ContainsKey(attribute.Id))
                    attribute.Equations = attribute.Equations.Concat(syncModel.AddedPairs[attribute.Id]).ToList();
                if (syncModel.DeletedPairs.ContainsKey(attribute.Id))
                    attribute.Equations = attribute.Equations.Where(_ => !syncModel.DeletedPairs[attribute.Id].Contains(_.Id)).ToList();
                if (syncModel.UpdatedPairs.ContainsKey(attribute.Id))
                {
                    var equations = attribute.Equations.ToList();
                    for (var o = 0; o < equations.Count; o++)
                    {
                        var eqation = syncModel.UpdatedPairs[attribute.Id].FirstOrDefault(row => row.Id == equations[o].Id);
                        if (eqation != null)
                            equations[o] = eqation;
                    }
                    attribute.Equations = equations;
                }
                    
            }

            return attributes;
        }

        private PagingPageModel<CalculatedAttributeEquationCriteriaPairDTO> HandlePaging(CalculatedAttributeDTO attribute, CalculatedAttributePagingRequestModel request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<CalculatedAttributeEquationCriteriaPairDTO>();
            var equations = attribute.Equations.ToList();
            if (request.search.Trim() != "")
                equations = SearchRows(equations, request.search);
            if (request.sortColumn.Trim() != "")
                equations = OrderByColumn(equations, request.sortColumn, request.isDescending);

            attribute = SyncedDataset(new List<CalculatedAttributeDTO>() { attribute }, request.SyncModel).First();

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = equations.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = equations;
                return new CalculcatedAttributePagingPageModel()
                {
                    CalculationTiming = attribute.CalculationTiming,
                    Items = items,
                    TotalItems = items.Count
                };
            }

            return new CalculcatedAttributePagingPageModel
            {
                CalculationTiming = attribute.CalculationTiming,
                Items = items,
                TotalItems = equations.Count()
            };
        }
    }
}
