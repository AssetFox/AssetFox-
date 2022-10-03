using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class EquationTestSetup
    {
        public static EquationEntity Two(Guid? id)
        {
            var resolveId = id ?? Guid.NewGuid();
            var equation = new EquationEntity
            {
                Expression = "2",
                Id = resolveId,
            };
            return equation;
        }

        public static EquationEntity TwoWithJoin(Guid? id, Guid performanceCurveId)
        {
            var equation = Two(id);
            var join = new PerformanceCurveEquationEntity
            {
                PerformanceCurveId = performanceCurveId,
                EquationId = equation.Id,
            };
            equation.PerformanceCurveEquationJoin = join;
            return equation;
        }

        public static EquationEntity TwoWithScenarioJoin(Guid? id, Guid performanceCurveId)
        {
            var equation = Two(id);
            var join = new ScenarioPerformanceCurveEquationEntity
            {
                EquationId = equation.Id,
                ScenarioPerformanceCurveId = performanceCurveId,
            };
            equation.ScenarioPerformanceCurveEquationJoin = join;
            return equation;
        }

        public static EquationEntity TwoWithJoinInDb(
            IUnitOfWork unitOfWork,
            Guid? id,
            Guid performanceCurveId)
        {
            var equation = TwoWithJoin(id, performanceCurveId);
            unitOfWork.Context.Add(equation);
            unitOfWork.Context.SaveChanges();
            return equation;
        }

        public static EquationEntity TwoWithScenarioJoinInDb(
            IUnitOfWork unitOfWork,
            Guid? id,
            Guid performanceCurveId)
        {
            var equation = TwoWithScenarioJoin(id, performanceCurveId);
            unitOfWork.Context.Add(equation);
            unitOfWork.Context.SaveChanges();
            return equation;
        }
    }
}
