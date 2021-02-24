using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestData
{
    public class PerformanceCurveTestHelper : TestHelper
    {
        public CriterionLibraryEntity CriterionLibraryEntity { get; } = new CriterionLibraryEntity
        {
            Id = Guid.Parse("f44fb6b2-a1f9-43a6-81e7-fbb4efa3fee5"),
            MergedCriteriaExpression = "Test"
        };

        public PerformanceCurveLibraryDTO LibraryToAdd { get; } = new PerformanceCurveLibraryDTO
        {
            Id = Guid.Parse("ef65d931-2a8f-417f-9687-0ee2cb563919"),
            Name = "Test Performance Curve Library",
            Description = "This is a test library",
            PerformanceCurves = new List<PerformanceCurveDTO>(),
            AppliedScenarioIds = new List<Guid>()
        };

        public PerformanceCurveLibraryDTO LibraryToUpdate { get; } = new PerformanceCurveLibraryDTO
        {
            Id = Guid.Parse("e357ad2c-f9a6-4b9c-a6b9-e9c1a2b0e896"),
            Name = "Test Performance Curve Library",
            Description = "This is a test library",
            PerformanceCurves = new List<PerformanceCurveDTO>(),
            AppliedScenarioIds = new List<Guid>()
        };

        public PerformanceCurveLibraryDTO LibraryToDelete { get; } = new PerformanceCurveLibraryDTO
        {
            Id = Guid.Parse("ef65d931-2a8f-417f-9687-0ee2cb563919"),
            Name = "Test Performance Curve Library",
            Description = "This is a test library",
            PerformanceCurves = new List<PerformanceCurveDTO>(),
            AppliedScenarioIds = new List<Guid>()
        };

        public PerformanceCurveDTO CurveToAdd { get; set; } = new PerformanceCurveDTO
        {
            Id = Guid.Parse("4448ba4d-f424-4034-8a99-be62382f8bc6"),
            Attribute = "",
            Shift = false,
            CriterionLibrary = new CriterionLibraryDTO(),
            Equation = new EquationDTO()
        };

        public PerformanceCurveDTO CurveToUpdate { get; set; } = new PerformanceCurveDTO
        {
            Id = Guid.Parse("568a9d9c-8ef0-4105-b5be-d272a1a29c09"),
            Attribute = "",
            Shift = false,
            CriterionLibrary = new CriterionLibraryDTO(),
            Equation = new EquationDTO()
        };

        public PerformanceCurveDTO CurveToDelete { get; set; } = new PerformanceCurveDTO
        {
            Id = Guid.Parse("5d347e0b-211e-4a6f-a2ee-7aef165475b5"),
            Attribute = "",
            Shift = false,
            CriterionLibrary = new CriterionLibraryDTO(),
            Equation = new EquationDTO()
        };

        public AttributeEntity GetAttribute() =>
            UnitOfDataPersistenceWork.Context.Attribute.First();

        public void SetupForGet()
        {
            CreateNetwork();
            CreateSimulation();
            
            UnitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Add(LibraryToAdd.ToEntity());

            var attribute = GetAttribute();
            CurveToAdd.Attribute = attribute.Name;
            UnitOfDataPersistenceWork.Context.PerformanceCurve.Add(CurveToAdd.ToEntity(LibraryToAdd.Id, attribute.Id));

            UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void SetupForPost()
        {
            CreateNetwork();
            CreateSimulation();

            UnitOfDataPersistenceWork.Context.CriterionLibrary.Add(CriterionLibraryEntity);

            UnitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Add(LibraryToUpdate.ToEntity());

            var attribute = GetAttribute();
            CurveToUpdate.Attribute = attribute.Name;
            CurveToDelete.Attribute = attribute.Name;
            var curveEntities = new List<PerformanceCurveEntity>
            {
                CurveToUpdate.ToEntity(LibraryToUpdate.Id, attribute.Id),
                CurveToDelete.ToEntity(LibraryToUpdate.Id, attribute.Id)
            };
            UnitOfDataPersistenceWork.Context.PerformanceCurve.AddRange(curveEntities);

            UnitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
