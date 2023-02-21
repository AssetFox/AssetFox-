using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeValueHistoryRepository : IAttributeValueHistoryRepository
    {
        private static readonly List<string> Props = new List<string>
        {
            "Id", "CreatedDate", "LastModifiedDate", "CreatedBy", "LastModifiedBy", "SectionId", "AttributeId", "Year", "Value"
        };

        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AttributeValueHistoryRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateNumericAttributeValueHistories(
            Dictionary<(Guid sectionId, Guid attributeId), IAttributeValueHistory<double>>
                numericAttributeValueHistoryPerSectionIdAttributeIdTuple)
        {
            var numericAttributeValueHistoryEntities = new List<NumericAttributeValueHistoryEntity>();

            numericAttributeValueHistoryPerSectionIdAttributeIdTuple.Keys.ForEach(tuple =>
            {
                var numericAttributeValueHistory = numericAttributeValueHistoryPerSectionIdAttributeIdTuple[tuple];
            });

            _unitOfWork.Context.AddAll(numericAttributeValueHistoryEntities, _unitOfWork.UserEntity?.Id);
        }

        public void CreateTextAttributeValueHistories(
            Dictionary<(Guid sectionId, Guid attributeId), IAttributeValueHistory<string>>
                textAttributeValueHistoryPerSectionIdAttributeIdTuple)
        {
            var textAttributeValueHistoryEntities = new List<TextAttributeValueHistoryEntity>();

            textAttributeValueHistoryPerSectionIdAttributeIdTuple.Keys.ForEach(key =>
            {
                var textAttributeValueHistory = textAttributeValueHistoryPerSectionIdAttributeIdTuple[key];
            });

            _unitOfWork.Context.AddAll(textAttributeValueHistoryEntities, _unitOfWork.UserEntity?.Id);
        }
    }

}
