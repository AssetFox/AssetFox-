﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AdminSettingEntity
    {
        public string ImplementationName { get; set; }
        public int ID { get; set; }
        public Image SiteLogo { get; set; }
        public Image ImplementationLogo { get; set; }
        public string PrimaryNetwork { get; set; }
        public string KeyFields { get; set; }
        public string InventoryReportNames { get; set; }
        public string SimulationReportNames { get; set; }
    }
}
