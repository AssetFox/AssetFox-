﻿using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class PerformanceCurveLibraryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<PerformanceCurveDTO> PerformanceCurves { get; set; } = new List<PerformanceCurveDTO>();
    }
}
