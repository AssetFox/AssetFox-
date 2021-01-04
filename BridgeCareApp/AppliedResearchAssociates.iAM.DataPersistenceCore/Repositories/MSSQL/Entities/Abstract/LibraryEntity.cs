﻿using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public abstract class LibraryEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}