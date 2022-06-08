using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CommittedProjectConsequenceMapper
    {
        public static CommittedProjectConsequenceEntity ToEntity(this TreatmentConsequence domain, Guid committedProjectId, Guid attributeId) =>
            new CommittedProjectConsequenceEntity
            {
                Id = domain.Id,
                CommittedProjectId = committedProjectId,
                AttributeId = attributeId,
                ChangeValue = domain.Change.Expression
            };

        public static void CreateCommittedProjectConsequence(this CommittedProjectConsequenceEntity entity, CommittedProject committedProject)
        {
            var consequence = committedProject.Consequences.GetAdd(new TreatmentConsequence());
            consequence.Id = entity.Id;
            consequence.Change.Expression = entity.ChangeValue;
            consequence.Attribute = committedProject.Asset.Network.Explorer.NumberAttributes
                .Single(_ => _.Name == entity.Attribute.Name);
        }
    }
}
