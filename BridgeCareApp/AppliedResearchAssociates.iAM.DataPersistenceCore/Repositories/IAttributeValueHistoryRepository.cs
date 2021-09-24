using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAttributeValueHistoryRepository
    {
        /*void CreateNumericAttributeValueHistories(List<((Guid sectionId, Guid attributeId) sectionIdAttributeId, AttributeValueHistory<double> numericAttributeValueHistory)> numericAttributeValueHistorySectionIdAttributeIdTupleTuples);
        void CreateTextAttributeValueHistories(List<((Guid sectionId, Guid attributeId) sectionIdAttributeId, AttributeValueHistory<string> textAttributeValueHistory)> textAttributeValueHistorySectionIdAttributeIdTupleTuples);*/

        void CreateNumericAttributeValueHistories(
            Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<double>>
                numericAttributeValueHistoryPerSectionIdAttributeIdTuple);

        void CreateTextAttributeValueHistories(
            Dictionary<(Guid sectionId, Guid attributeId), AttributeValueHistory<string>>
                numericAttributeValueHistoryPerSectionIdAttributeIdTuple);
    }
}
