using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class CommittedProjectConsequenceMapper
    {
        public static CommittedProjectConsequenceEntity ToEntity(this TreatmentConsequence domain, Guid committedProjectId, Guid attributeId) =>
            new CommittedProjectConsequenceEntity
            {
                Id = Guid.NewGuid(),
                CommittedProjectId = committedProjectId,
                AttributeId = attributeId,
                ChangeValue = domain.Change.Expression
            };

        public static void CreateCommittedProjectConsequence(this CommittedProjectConsequenceEntity entity, CommittedProject committedProject)
        {
            var consequence = committedProject.Consequences.GetAdd(new TreatmentConsequence());
            consequence.Change.Expression = entity.ChangeValue;
            consequence.Attribute = committedProject.Section.Facility.Network.Explorer.NumberAttributes
                .Single(_ => _.Name == entity.Attribute.Name);
        }
    }
}
