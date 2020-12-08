using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SectionRepository : MSSQLRepository, ISectionRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly IAttributeValueHistoryRepository _attributeValueHistoryRepo;

        public SectionRepository(IAttributeValueHistoryRepository attributeValueHistoryRepo, IAMContext context) : base(context) =>
            _attributeValueHistoryRepo = attributeValueHistoryRepo ?? throw new ArgumentNullException(nameof(attributeValueHistoryRepo));

        public void CreateSections(Dictionary<Guid, List<Section>> sectionsPerFacilityId)
        {
            var attributeNames = sectionsPerFacilityId.SelectMany(_ => _.Value
                    .SelectMany(__ => __.HistoricalAttributes
                        .Select(___ => ___.Name)))
                .Distinct().ToList();

            var attributeEntities = Context.Attribute
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

            var sectionEntities = sectionsPerFacilityId.SelectMany(_ =>
            {
                var (facilityId, sections) = _;
                return sections.Select(section =>
                    {
                        var entity = section.ToEntity(facilityId);

                        if (section.HistoricalAttributes.Any())
                        {
                            section.HistoricalAttributes.ForEach(attribute =>
                            {
                                if (attribute is NumberAttribute numberAttribute)
                                {
                                    numericAttributeValueHistoryPerSectionIdAttributeIdTuple.Add(
                                        (entity.Id, attributeIdPerName[numberAttribute.Name]), section.GetHistory(numberAttribute)
                                    );
                                }

                                if (attribute is TextAttribute textAttribute)
                                {
                                    textAttributeValueHistoryPerSectionIdAttributeIdTuple.Add(
                                        (entity.Id, attributeIdPerName[textAttribute.Name]), section.GetHistory(textAttribute)
                                    );
                                }
                            });
                        }

                        return entity;
                    });
            }).ToList();

            if (IsRunningFromXUnit)
            {
                Context.Section.AddRange(sectionEntities);
            }
            else
            {
                Context.BulkInsert(sectionEntities);
            }

            Context.SaveChanges();

            if (numericAttributeValueHistoryPerSectionIdAttributeIdTuple.Any())
            {
                _attributeValueHistoryRepo.CreateNumericAttributeValueHistories(numericAttributeValueHistoryPerSectionIdAttributeIdTuple);
            }

            if (textAttributeValueHistoryPerSectionIdAttributeIdTuple.Any())
            {
                _attributeValueHistoryRepo.CreateTextAttributeValueHistories(textAttributeValueHistoryPerSectionIdAttributeIdTuple);
            }
        }
    }
}
