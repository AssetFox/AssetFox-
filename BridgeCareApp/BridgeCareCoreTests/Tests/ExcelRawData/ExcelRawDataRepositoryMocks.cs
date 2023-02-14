﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Moq;

namespace BridgeCareCoreTests.Tests
{
    public static class ExcelRawDataRepositoryMocks
    {
        public static Mock<IExcelRawDataRepository> New(Mock<IUnitOfWork> unitOfWork = null)
        {
            var mock = new Mock<IExcelRawDataRepository>();
            if (unitOfWork != null)
            {
                unitOfWork.Setup(u => u.ExcelWorksheetRepository).Returns(mock.Object);
            }
            return mock;
        }
    }
}