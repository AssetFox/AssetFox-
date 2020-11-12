﻿using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SimulationOutputEntity
    {
        public Guid Id { get; set; }

        public byte[] Output { get; set; }

        public Guid SimulationId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }
    }
}
