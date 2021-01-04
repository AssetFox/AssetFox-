﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SectionRepository : ISectionRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public SectionRepository(UnitOfWork.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateSections(List<Section> sections)
        {
            var attributeNames = sections
                .SelectMany(_ => _.HistoricalAttributes.Select(__ => __.Name))
                .Distinct().ToList();

            var attributeEntities = _unitOfWork.Context.Attribute
                .Where(_ => attributeNames.Contains(_.Name)).ToList();

            if (!attributeEntities.Any())
            {
                throw new RowNotInTableException("No attributes found for section attribute value histories.");
            }

            var attributeNamesFromDataSource = attributeEntities.Select(_ => _.Name).ToList();
            if (!attributeNames.All(attributeName => attributeNamesFromDataSource.Contains(attributeName)))
            {
                var attributeNamesNotFound = attributeNames.Except(attributeNamesFromDataSource).ToList();
                if (attributeNamesNotFound.Count() == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {attributeNamesNotFound[0]}.");
                }

                throw new RowNotInTableException($"No attributes found having names: {string.Join(", ", attributeNamesNotFound)}.");
            }

            var attributeIdPerName = attributeEntities.ToDictionary(_ => _.Name, _ => _.Id);

            var numericAttributeValueHistoryPerSectionIdAttributeIdTuple = new Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<double>>();
            var textAttributeValueHistoryPerSectionIdAttributeIdTuple = new Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<string>>();

            var sectionEntities = sections.Select(_ =>
            {
                var entity = _.ToEntity();

                if (_.HistoricalAttributes.Any())
                {
                    _.HistoricalAttributes.ForEach(attribute =>
                    {
                        if (attribute is NumberAttribute numberAttribute)
                        {
                            numericAttributeValueHistoryPerSectionIdAttributeIdTuple.Add(
                                (_.Id, attributeIdPerName[numberAttribute.Name]), _.GetHistory(numberAttribute)
                            );
                        }

                        if (attribute is TextAttribute textAttribute)
                        {
                            textAttributeValueHistoryPerSectionIdAttributeIdTuple.Add(
                                (_.Id, attributeIdPerName[textAttribute.Name]), _.GetHistory(textAttribute)
                            );
                        }
                    });
                }

                return entity;
            }).ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.Section.AddRange(sectionEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsert(sectionEntities);
            }

            _unitOfWork.Context.SaveChanges();

            if (numericAttributeValueHistoryPerSectionIdAttributeIdTuple.Any())
            {
                _unitOfWork.AttributeValueHistoryRepo.CreateNumericAttributeValueHistories(numericAttributeValueHistoryPerSectionIdAttributeIdTuple);
            }

            if (textAttributeValueHistoryPerSectionIdAttributeIdTuple.Any())
            {
                _unitOfWork.AttributeValueHistoryRepo.CreateTextAttributeValueHistories(textAttributeValueHistoryPerSectionIdAttributeIdTuple);
            }
        }
    }
}