using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using MoreLinq;
using Attribute = AppliedResearchAssociates.iAM.Domains.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeValueHistoryRepository : MSSQLRepository, IAttributeValueHistoryRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public AttributeValueHistoryRepository(IAMContext context) : base(context) { }

        public void CreateNumericAttributeValueHistories(
            Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<double>>
                numericAttributeValueHistoryPerSectionIdAttributeIdTuple)
        {
            //var numericAttributeValueHistoryMostRecentValueEntities = new List<NumericAttributeValueHistoryMostRecentValueEntity>();
            var numericAttributeValueHistoryEntities = new List<NumericAttributeValueHistoryEntity>();

            numericAttributeValueHistoryPerSectionIdAttributeIdTuple.Keys.ForEach(tuple =>
            {
                var numericAttributeValueHistory = numericAttributeValueHistoryPerSectionIdAttributeIdTuple[tuple];
                numericAttributeValueHistoryEntities.AddRange(numericAttributeValueHistory.ToEntity(tuple.sectionId, tuple.attributeId));
            });

            if (IsRunningFromXUnit)
            {
                /*Context.NumericAttributeValueHistoryMostRecentValue.AddRange(
                    numericAttributeValueHistoryMostRecentValueEntities);*/
                Context.NumericAttributeValueHistory.AddRange(numericAttributeValueHistoryEntities);
            }
            else
            {
                //Context.BulkInsert(numericAttributeValueHistoryMostRecentValueEntities);
                Context.BulkInsert(numericAttributeValueHistoryEntities);
            }

            Context.SaveChanges();
        }

        public void CreateTextAttributeValueHistories(
            Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<string>>
                textAttributeValueHistoryPerSectionIdAttributeIdTuple)
        {
            //var textAttributeValueHistoryMostRecentValueEntities = new List<TextAttributeValueHistoryMostRecentValueEntity>();
            var textAttributeValueHistoryEntities = new List<TextAttributeValueHistoryEntity>();

            textAttributeValueHistoryPerSectionIdAttributeIdTuple.Keys.ForEach(key =>
            {
                var textAttributeValueHistory = textAttributeValueHistoryPerSectionIdAttributeIdTuple[key];
                textAttributeValueHistoryEntities.AddRange(textAttributeValueHistory.ToEntity(key.sectionId, key.attributeId));
            });

            if (IsRunningFromXUnit)
            {
                /*Context.TextAttributeValueHistoryMostRecentValue.AddRange(
                    textAttributeValueHistoryMostRecentValueEntities);*/
                Context.TextAttributeValueHistory.AddRange(textAttributeValueHistoryEntities);
            }
            else
            {
                //Context.BulkInsert(textAttributeValueHistoryMostRecentValueEntities);
                Context.BulkInsert(textAttributeValueHistoryEntities);
            }

            Context.SaveChanges();
        }
    }
}
