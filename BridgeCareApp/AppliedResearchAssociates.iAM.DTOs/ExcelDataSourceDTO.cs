using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class ExcelDataSourceDTO : BaseDataSourceDTO
    {
        public ExcelDataSourceDTO() : base(DataSourceTypeStrings.Excel.ToString())
        {
            Secure = false;
        }

        public string LocationColumn { get; set; }
        public string DateColumn { get; set; }
        public override bool Validate() => true;
    }
}
