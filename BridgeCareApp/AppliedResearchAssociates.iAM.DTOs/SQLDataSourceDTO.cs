using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SQLDataSourceDTO : BaseDataSourceDTO
    {
        public SQLDataSourceDTO() : base(DataSourceTypeStrings.SQL.ToString())
        {
            Secure = true;
        }

        public string ConnectionString { get; set; }

        public override string MapDetails() => ConnectionString;

        public override void PopulateDetails(string details)
        {
            ConnectionString = details;
        }

        public override bool Validate() => true;
    }
}
