using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace BridgeCareCore.Models
{
    public class AllDataSource : BaseDataSourceDTO
    {
        public AllDataSource() : base(DataSourceTypeStrings.All.ToString())
        {
            Secure = false;
        }

        public string ConnectionString { get; set; }
        public string LocationColumn { get; set; }
        public string DateColumn { get; set; }

        public override bool Validate() => true;
    }
}
