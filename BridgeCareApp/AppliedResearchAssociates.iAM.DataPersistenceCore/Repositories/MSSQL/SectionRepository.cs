using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SectionRepository : ISectionRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public SectionRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateSections(List<Section> sections)
        {
            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            var sectionAttributeNames = sections
                .SelectMany(_ => _.HistoricalAttributes.Select(__ => __.Name))
                .Distinct().ToList();
            if (!sectionAttributeNames.All(sectionAttributeName => attributeNames.Contains(sectionAttributeName)))
            {
                var missingAttributes = sectionAttributeNames.Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having names: {string.Join(", ", missingAttributes)}.");
            }

            var attributeIdPerName = attributeEntities.ToDictionary(_ => _.Name, _ => _.Id);

            var numericAttributeValueHistoryPerSectionIdAttributeIdTuple =
                new Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<double>>();
            var textAttributeValueHistoryPerSectionIdAttributeIdTuple =
                new Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<string>>();

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

            _unitOfWork.Context.AddAll(sectionEntities, _unitOfWork.UserEntity?.Id);

            if (numericAttributeValueHistoryPerSectionIdAttributeIdTuple.Any())
            {
                _unitOfWork.AttributeValueHistoryRepo.CreateNumericAttributeValueHistories(
                    numericAttributeValueHistoryPerSectionIdAttributeIdTuple);
            }

            if (textAttributeValueHistoryPerSectionIdAttributeIdTuple.Any())
            {
                _unitOfWork.AttributeValueHistoryRepo.CreateTextAttributeValueHistories(
                    textAttributeValueHistoryPerSectionIdAttributeIdTuple);
            }
        }
    }
}
