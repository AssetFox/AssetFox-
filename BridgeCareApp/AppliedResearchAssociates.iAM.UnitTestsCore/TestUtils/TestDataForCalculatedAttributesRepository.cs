using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class TestDataForCalculatedAttributesRepository
    {
        public static Guid FirstSimulationId => new Guid("440ab4bb-da87-4aee-868b-4272910fae9b");
        public static Guid SecondSimulationId => new Guid("bf8984b8-4fbf-4b54-9035-b20b349a6808");
        public static IQueryable<SimulationEntity> GetSimulations()
        {
            var simulationList = new List<SimulationEntity>();
            simulationList.Add(new SimulationEntity()
            {
                Id = FirstSimulationId,
                Name = "First"
            });
            simulationList.Add(new SimulationEntity()
            {
                Id = SecondSimulationId,
                Name = "Second"
            });

            return simulationList.AsQueryable();
        }

        public static IQueryable<AttributeEntity> GetAttributeRepo()
        {
            var attributeList = new List<AttributeEntity>();
            attributeList.Add(new AttributeEntity()
            {
                Id = new Guid("87ad4cb8-ea2e-49d9-b588-60803427de9c"),
                Name = "AGE",
                DataType = "NUMBER",
                IsCalculated = false
            });
            attributeList.Add(new AttributeEntity()
            {
                Id = new Guid("1a33bdf2-08e4-4456-861b-059a73139613"),
                Name = "CONDITION",
                DataType = "NUMBER",
                IsCalculated = true
            });
            attributeList.Add(new AttributeEntity()
            {
                Id = new Guid("eb80878a-2cfa-49ec-b375-e34bdf38bf66"),
                Name = "DESCRIPTION",
                DataType = "STRING",
                IsCalculated = false
            });

            return attributeList.AsQueryable();
        }

        public static IQueryable<CalculatedAttributeLibraryEntity> GetLibraryRepo()
        {
            var result = new List<CalculatedAttributeLibraryEntity>();
            var attributes = GetAttributeRepo();

            var library = new CalculatedAttributeLibraryEntity();
            library.Id = new Guid("86bf65df-5ac9-44cc-b26d-9a1182c258d4");
            library.Name = "First";
            library.CalculatedAttributes = new List<CalculatedAttributeEntity>();
            library.IsDefault = true;

            int loopCount = 1;
            foreach (var attribute in attributes)
            {
                var newCalculation = CreateNewCalculatedAttribute(attribute, $"[{attribute.Name}] + 1");
                if (loopCount > 1) AddEquation(newCalculation, $"[{attribute.Name}] + 2", "[Status] = 'Good'");
                newCalculation.CalculatedAttributeLibraryId = library.Id;
                library.CalculatedAttributes.Add(newCalculation);
                loopCount++;
            }

            result.Add(library);

            library = new CalculatedAttributeLibraryEntity();
            library.Id = new Guid("d9df3586-3103-4e65-b7f4-0a986eae6989");
            library.Name = "Second";
            library.CalculatedAttributes = new List<CalculatedAttributeEntity>();

            loopCount = 1;
            foreach (var attribute in attributes)
            {
                var newCalculation = CreateNewCalculatedAttribute(attribute, $"[{attribute.Name}] - 1");
                if (loopCount == 1) AddEquation(newCalculation, $"[{attribute.Name}] - 2", "[Status] = 'Bad'");
                newCalculation.CalculatedAttributeLibraryId = library.Id;
                library.CalculatedAttributes.Add(newCalculation);
                loopCount++;
            }

            result.Add(library);

            return result.AsQueryable();
        }

        public static IQueryable<ScenarioCalculatedAttributeEntity> GetSimulationCalculatedAttributesRepo(bool ignoreIsCalculated = true)
        {
            var result = new List<ScenarioCalculatedAttributeEntity>();
            var scenarios = GetSimulations();
            var attributes = GetAttributeRepo();

            if (!ignoreIsCalculated)
            {
                attributes = GetAttributeRepo().Where(_ => _.IsCalculated);
            }

            var selectedScenario = scenarios.First(_ => _.Name == "First");
            foreach (var attribute in attributes)
            {
                var newCalculation = CreateNewCalculatedAttribute(attribute, selectedScenario, $"[{attribute.Name}] + 1");
                AddEquation(newCalculation, "[{attribute.Name}] + 2", "[Status] = 'Good'");
                result.Add(newCalculation);
            }

            selectedScenario = scenarios.First(_ => _.Name == "Second");
            var loopCount = 1;
            foreach (var attribute in attributes)
            {
                var newCalculation = CreateNewCalculatedAttribute(attribute, selectedScenario, $"[{attribute.Name}] - 5");
                if (loopCount > 1) AddEquation(newCalculation, "[{attribute.Name}] - 10", "[Status] = 'Bad'");
                result.Add(newCalculation);
                loopCount++;
            }

            return result.AsQueryable();
        }


        public static CalculatedAttributeEntity CreateNewCalculatedAttribute(AttributeEntity attribute, string firstEquation, int calculationTiming = 1)
        {
            var result = new CalculatedAttributeEntity
            {
                Id = Guid.NewGuid(),
                CalculationTiming = calculationTiming,
                AttributeId = attribute.Id,
                Attribute = attribute
            };

            var firstPair = new CalculatedAttributeEquationCriteriaPairEntity
            {
                Id = Guid.NewGuid(),
                CalculatedAttributeId = result.Id,
                CalculatedAttribute = result
            };

            var equationCalculationJoin = new EquationCalculatedAttributePairEntity
            {
                CalculatedAttributePairId = firstPair.Id,
                CalculatedAttributePair = firstPair
            };
            firstPair.EquationCalculatedAttributeJoin = equationCalculationJoin;

            var equationObject = new EquationEntity
            {
                Id = Guid.NewGuid(),
                Expression = firstEquation,
                CalculatedAttributePairJoin = equationCalculationJoin
            };
            equationCalculationJoin.EquationId = equationObject.Id;
            equationCalculationJoin.Equation = equationObject;

            result.Equations.Add(firstPair);

            return result;
        }

        public static void AddEquation(CalculatedAttributeEntity calculatedAttribute, string equation, string criteria)
        {
            var addedPair = new CalculatedAttributeEquationCriteriaPairEntity
            {
                Id = Guid.NewGuid(),
                CalculatedAttributeId = calculatedAttribute.Id,
                CalculatedAttribute = calculatedAttribute
            };

            var equationCalculationJoin = new EquationCalculatedAttributePairEntity
            {
                CalculatedAttributePairId = addedPair.Id,
                CalculatedAttributePair = addedPair
            };
            addedPair.EquationCalculatedAttributeJoin = equationCalculationJoin;
            var equationObject = new EquationEntity
            {
                Id = Guid.NewGuid(),
                Expression = equation,
                CalculatedAttributePairJoin = equationCalculationJoin
            };
            equationCalculationJoin.EquationId = equationObject.Id;
            equationCalculationJoin.Equation = equationObject;

            var criteriaCalculationJoin = new CriterionLibraryCalculatedAttributePairEntity
            {
                CalculatedAttributePairId = addedPair.Id,
                CalculatedAttributePair = addedPair
            };
            addedPair.CriterionLibraryCalculatedAttributeJoin = criteriaCalculationJoin;
            var criteriaObject = new CriterionLibraryEntity
            {
                Id = Guid.NewGuid(),
                Name = $"Criteria for {criteria}",
                MergedCriteriaExpression = criteria,
                IsSingleUse = false,
                CriterionLibraryCalculatedAttributePairJoins = new List<CriterionLibraryCalculatedAttributePairEntity>()
            };
            criteriaObject.CriterionLibraryCalculatedAttributePairJoins.Add(criteriaCalculationJoin);
            criteriaCalculationJoin.CriterionLibraryId = criteriaObject.Id;
            criteriaCalculationJoin.CriterionLibrary = criteriaObject;

            calculatedAttribute.Equations.Add(addedPair);
        }

        public static ScenarioCalculatedAttributeEntity CreateNewCalculatedAttribute(AttributeEntity attribute, SimulationEntity scenario, string firstEquation, int calculationTiming = 1)
        {
            var result = new ScenarioCalculatedAttributeEntity
            {
                Id = Guid.NewGuid(),
                CalculationTiming = calculationTiming,
                AttributeId = attribute.Id,
                Attribute = attribute,
                SimulationId = scenario.Id,
                Simulation = scenario
            };

            var firstPair = new ScenarioCalculatedAttributeEquationCriteriaPairEntity
            {
                Id = Guid.NewGuid(),
                ScenarioCalculatedAttributeId = result.Id,
                ScenarioCalculatedAttribute = result
            };

            var equationCalculationJoin = new ScenarioEquationCalculatedAttributePairEntity
            {
                ScenarioCalculatedAttributePairId = firstPair.Id,
                ScenarioCalculatedAttributePair = firstPair
            };
            firstPair.EquationCalculatedAttributeJoin = equationCalculationJoin;

            var equationObject = new EquationEntity
            {
                Id = Guid.NewGuid(),
                Expression = firstEquation,
                ScenarioCalculatedAttributePairJoin = equationCalculationJoin
            };
            equationCalculationJoin.EquationId = equationObject.Id;
            equationCalculationJoin.Equation = equationObject;

            result.Equations.Add(firstPair);

            return result;
        }

        public static void AddEquation(ScenarioCalculatedAttributeEntity calculatedAttribute, string equation, string criteria)
        {
            var addedPair = new ScenarioCalculatedAttributeEquationCriteriaPairEntity
            {
                Id = Guid.NewGuid(),
                ScenarioCalculatedAttributeId = calculatedAttribute.Id,
                ScenarioCalculatedAttribute = calculatedAttribute
            };

            var equationCalculationJoin = new ScenarioEquationCalculatedAttributePairEntity
            {
                ScenarioCalculatedAttributePairId = addedPair.Id,
                ScenarioCalculatedAttributePair = addedPair
            };
            addedPair.EquationCalculatedAttributeJoin = equationCalculationJoin;
            var equationObject = new EquationEntity
            {
                Id = Guid.NewGuid(),
                Expression = equation,
                ScenarioCalculatedAttributePairJoin = equationCalculationJoin
            };
            equationCalculationJoin.EquationId = equationObject.Id;
            equationCalculationJoin.Equation = equationObject;

            var criteriaCalculationJoin = new ScenarioCriterionLibraryCalculatedAttributePairEntity
            {
                ScenarioCalculatedAttributePairId = addedPair.Id,
                ScenarioCalculatedAttributePair = addedPair
            };
            addedPair.CriterionLibraryCalculatedAttributeJoin = criteriaCalculationJoin;
            var criteriaObject = new CriterionLibraryEntity
            {
                Id = Guid.NewGuid(),
                Name = $"Criteria for {criteria}",
                MergedCriteriaExpression = criteria,
                IsSingleUse = false,
                CriterionLibraryCalculatedAttributePairJoins = new List<CriterionLibraryCalculatedAttributePairEntity>()
            };
            criteriaObject.CriterionLibraryScenarioCalculatedAttributePairJoins.Add(criteriaCalculationJoin);
            criteriaCalculationJoin.CriterionLibraryId = criteriaObject.Id;
            criteriaCalculationJoin.CriterionLibrary = criteriaObject;

            calculatedAttribute.Equations.Add(addedPair);
        }
    }
}
