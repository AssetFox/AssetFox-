using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services.Paging.Generics;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace BridgeCareCore.Services
{
    public class CalculatedAttributePagingService : ICalculatedAttributePagingService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CalculatedAttributePagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public PagingPageModel<CalculatedAttributeEquationCriteriaPairDTO> GetScenarioPage(Guid libraryId, CalculatedAttributePagingRequestModel request)
        {
            var calcAttribute = new CalculatedAttributeDTO();
            var attribute = _unitOfWork.AttributeRepo.GetSingleById(request.AttributeId);
            var addedCalc = request.SyncModel.AddedCalculatedAttributes.FirstOrDefault(_ => _.Attribute == attribute.Name);
            if (addedCalc != null)
                calcAttribute = addedCalc;
            else
                calcAttribute = _unitOfWork.CalculatedAttributeRepo.GetLibraryCalulatedAttributesByLibraryAndAttributeId(libraryId, request.AttributeId);
            return HandlePaging(calcAttribute, request);
        }

        public PagingPageModel<CalculatedAttributeEquationCriteriaPairDTO> GetLibraryPage(Guid simulationId, CalculatedAttributePagingRequestModel request)
        {
            var calcAttribute = new CalculatedAttributeDTO();
            var attribute = _unitOfWork.AttributeRepo.GetSingleById(request.AttributeId);
            var addedCalc = request.SyncModel.AddedCalculatedAttributes.FirstOrDefault(_ => _.Attribute == attribute.Name);
            if (addedCalc != null)
                calcAttribute = addedCalc;
            else
                calcAttribute = request.SyncModel.LibraryId == null ? _unitOfWork.CalculatedAttributeRepo.GetScenarioCalulatedAttributesByScenarioAndAttributeId(simulationId, request.AttributeId):
                _unitOfWork.CalculatedAttributeRepo.GetLibraryCalulatedAttributesByLibraryAndAttributeId(request.SyncModel.LibraryId.Value, request.AttributeId);

            return HandlePaging(calcAttribute, request);
        }

        public List<CalculatedAttributeDTO> GetSyncedScenarioDataSet(Guid simulationId, CalculatedAttributePagingSyncModel request)
        {
            var attributes = request.LibraryId == null ?
                    _unitOfWork.CalculatedAttributeRepo.GetScenarioCalculatedAttributes(simulationId).ToList() :
                    _unitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraryByID(request.LibraryId.Value).CalculatedAttributes.ToList();
            attributes = attributes.Concat(request.AddedCalculatedAttributes).ToList();
            attributes = SyncedDataset(attributes, request);
            if (request.LibraryId != null)
            {
                attributes.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.Equations.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        if(__.CriteriaLibrary != null)
                            __.CriteriaLibrary.Id = Guid.NewGuid();
                        if (__.Equation != null)
                            __.Equation.Id = Guid.NewGuid();
                    });
                });
            }

            return attributes;
        }

        public List<CalculatedAttributeDTO> GetSyncedLibraryDataset(Guid libraryId, CalculatedAttributePagingSyncModel request)
        {
            var library = _unitOfWork.CalculatedAttributeRepo.GetCalculatedAttributeLibraryByID(libraryId);
            library.CalculatedAttributes = library.CalculatedAttributes.Concat(request.AddedCalculatedAttributes).ToList(); 
            return SyncedDataset(library.CalculatedAttributes.ToList(), request);
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
            case "criteriaexpression":
                if (isDescending)
                    return equations.OrderByDescending(_ => _.CriteriaLibrary.MergedCriteriaExpression.ToLower()).ToList();
                else
                    return equations.OrderBy(_ => _.CriteriaLibrary.MergedCriteriaExpression.ToLower()).ToList();
            }
            return equations;
        }

        private List<CalculatedAttributeEquationCriteriaPairDTO> SearchRows(List<CalculatedAttributeEquationCriteriaPairDTO> equations, string search)
        {
            var lowerCaseSearch = search.ToLower();
            return equations
                .Where(_ => (_.Equation.Expression != null && _.Equation.Expression.ToLower().Contains(lowerCaseSearch)) ||
                    (_.CriteriaLibrary.MergedCriteriaExpression != null && _.CriteriaLibrary.MergedCriteriaExpression.ToLower().Contains(lowerCaseSearch))).ToList();
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
                if(syncModel.DefaultEquations.ContainsKey(attribute.Id))
                {
                    var equations = attribute.Equations.ToList();
                    var defaultEquation = equations.FirstOrDefault(_ => _.Id == syncModel.DefaultEquations[attribute.Id].Id);
                    if(defaultEquation != null)
                    {
                        equations[equations.FindIndex(_ => _.Id == defaultEquation.Id)] = syncModel.DefaultEquations[attribute.Id];
                    }
                    else
                        equations.Add(syncModel.DefaultEquations[attribute.Id]);

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

            CalculatedAttributeEquationCriteriaPairDTO defaultEquation = null;
            var equations = attribute.Equations.ToList();

            if (request.SyncModel.DefaultEquations.ContainsKey(attribute.Id))
            {
                defaultEquation = request.SyncModel.DefaultEquations[attribute.Id];
            }
            else
            {
                defaultEquation = equations.FirstOrDefault(_ => (_.Equation != null && _.Equation.Expression.Trim() != "") &&
                (_.CriteriaLibrary == null || _.CriteriaLibrary.MergedCriteriaExpression.Trim() == ""));
                if (defaultEquation == null)
                {
                    defaultEquation = new CalculatedAttributeEquationCriteriaPairDTO();
                    defaultEquation.Id = Guid.NewGuid();
                    defaultEquation.Equation = new EquationDTO();
                    defaultEquation.Equation.Id = Guid.NewGuid();
                    defaultEquation.Equation.Expression = "";
                }
                else
                {
                    equations.Remove(defaultEquation);
                    attribute.Equations = equations;
                }
            }
                                                
            attribute = SyncedDataset(new List<CalculatedAttributeDTO>() { attribute }, request.SyncModel).First();

            equations = attribute.Equations.ToList();

            if(request.SyncModel.DefaultEquations.ContainsKey(attribute.Id))
                equations.RemoveAt(equations.FindIndex(_ => _.Id == defaultEquation.Id));

            if (request.search != null && request.search.Trim() != "")
                equations = SearchRows(equations, request.search);
            if (request.sortColumn != null && request.sortColumn.Trim() != "")
                equations = OrderByColumn(equations, request.sortColumn, request.isDescending);

            attribute.Equations = equations;

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                items = attribute.Equations.Skip(skip).Take(take).ToList();
            }
            else
            {
                items = attribute.Equations.ToList();
                return new CalculcatedAttributePagingPageModel()
                {
                    CalculationTiming = attribute.CalculationTiming,
                    Items = items,
                    TotalItems = attribute.Equations.Count,
                    DefaultEquation = defaultEquation
                };
            }

            return new CalculcatedAttributePagingPageModel
            {
                CalculationTiming = attribute.CalculationTiming,
                Items = items,
                TotalItems = attribute.Equations.Count,
                DefaultEquation = defaultEquation
            };
        }
    }
}
