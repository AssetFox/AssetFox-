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
using BridgeCareCore.Services.Paging.Generics;
using AppliedResearchAssociates.iAM.Analysis;
using Org.BouncyCastle.Asn1.Ocsp;

namespace BridgeCareCore.Services
{
    public class PerformanceCurvesPagingService : PagingService<PerformanceCurveDTO, PerformanceCurveLibraryDTO> , IPerformanceCurvesPagingService
    {
        private static IUnitOfWork _unitOfWork;        

        public PerformanceCurvesPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }



        protected override List<PerformanceCurveDTO> OrderByColumn(List<PerformanceCurveDTO> curves, string sortColumn, bool isDescending)
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

        protected override List<PerformanceCurveDTO> SearchRows(List<PerformanceCurveDTO> curves, string search)
        {
            var lowerCaseSearch = search.ToLower();
            return curves
                .Where(_ => _.Name.ToLower().Contains(lowerCaseSearch) ||
                    _.Attribute.ToLower().Contains(lowerCaseSearch) ||
                    (_.Equation.Expression != null && _.Equation.Expression.ToLower().Contains(lowerCaseSearch)) ||
                    (_.CriterionLibrary.MergedCriteriaExpression != null && _.CriterionLibrary.MergedCriteriaExpression.ToLower().Contains(lowerCaseSearch))).ToList();
        }

        protected override List<PerformanceCurveDTO> GetScenarioRows(Guid scenarioId) => _unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurvesOrderedById(scenarioId);

        protected override List<PerformanceCurveDTO> GetLibraryRows(Guid libraryId) => _unitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibraryOrderedById(libraryId);
        protected override List<PerformanceCurveDTO> CreateAsNewDataset(List<PerformanceCurveDTO> rows)
        {
            rows.ForEach(_ =>
            {
                _.Id = Guid.NewGuid();
                _.CriterionLibrary.Id = Guid.NewGuid();
            });

            return rows;
        }
    }
}
