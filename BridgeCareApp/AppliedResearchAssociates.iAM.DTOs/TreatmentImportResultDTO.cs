﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TreatmentImportResultDTO: WarningServiceResultDTO
    {
        public TreatmentLibraryDTO TreatmentLibrary { get; set; }
    }
}