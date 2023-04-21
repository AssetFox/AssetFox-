using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Reporting.Models.PAMSPBExport
{
    public class MASDataModel
    {
        public Guid NetworkId { get; set; }

        public Guid MaintainableAssetId { get; set; }

        public string District { get; set; }

        public string Cnty { get; set; }

        public string Route { get; set; }

        public string AssetName { get; set; }

        public string Direction { get; set; }

        public string FromSection { get; set; }

        public string ToSection { get; set; }        

        public double Area { get; set; }

        public string Interstate { get; set; }

        public double Lanes { get; set; }

        public double Width { get; set; }

        public double Length { get; set; }

        public string surfaceName { get; set; } 

        public double RiskScore { get; set; } 
    }
}
