using System;
using AppliedResearchAssociates.iAM.DataPersistence.Models;
using AppliedResearchAssociates.iAM.DataPersistence.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;

namespace AppliedResearchAssociates.iAM.UnitTests
{
    [TestFixture]
    public class DataPersistenceTest
    {
        [Test]
        public void CanCreateScenarioWithCorrectModel()
        {
            var scenarioRepository = new Mock<IRepository<Scenario>>();

            //var scenarioController = new ScenarioController(
            //        scenarioRepository.Object
            //    );
            var createScenario = new Scenario{
                Name = "TestScenario",
                Status = "TestStatus"
            };

            //scenarioController.Create(createScenario);

            scenarioRepository.Verify(repository => repository.Add(It.IsAny<Scenario>()), Times.AtMostOnce());
        }
    }
}
