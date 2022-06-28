using System.Text.Json;
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

        public override string MapDetails()
        {
            var details = new ExcelDataSourceDetails { LocationColumn = this.LocationColumn, DateColumn = this.DateColumn };
            return JsonSerializer.Serialize(details);
        }
        public override void PopulateDetails(string details)
        {
            var hydrated = JsonSerializer.Deserialize<ExcelDataSourceDetails>(details);
            LocationColumn = hydrated.LocationColumn;
            DateColumn = hydrated.DateColumn;
        }

        private class ExcelDataSourceDetails
        {
            public string LocationColumn { get; set; }
            public string DateColumn { get; set; }
        }

        public override bool Validate() => true;
    }

    
}
