using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.TestHelpers;
namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class CommittedProjectTetsSetup
    {
        public static CommittedProjectEntity TestCommittedProject(Guid projectId, Guid simulationId, Guid? budgetId = null, string treatment = "test treatment")
        {
            return new CommittedProjectEntity()
            {
                Id = projectId,
                ScenarioBudgetId = budgetId,
                SimulationId = simulationId,
                Category = TreatmentCategory.Preservation.ToString(),
                Year = 1995,
                ShadowForAnyTreatment = 0,
                ShadowForSameTreatment = 0,
                Name = treatment,
                Cost = 0,
                CommittedProjectLocation = new CommittedProjectLocationEntity()
                {
                    Discriminator = DataPersistenceConstants.SectionLocation,
                    Id = Guid.NewGuid(),
                    LocationIdentifier = "BRKEY_",
                }
            };
        }

        public static CommittedProjectEntity TestCommittedProjectInDb(IUnitOfWork unitOfWork, Guid projectId, Guid simulationId, Guid? budgetId = null, string treatment = "test treatment")
        {
            var project = TestCommittedProject(projectId, simulationId, budgetId, treatment);
            unitOfWork.Context.CommittedProject.Add(project);
            unitOfWork.Context.SaveChanges();
            return project;
        }

    }
}
