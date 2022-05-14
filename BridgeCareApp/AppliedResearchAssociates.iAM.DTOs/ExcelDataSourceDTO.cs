﻿using System.Text.Json;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class ExcelDataSourceDTO : BaseDataSourceDTO
    {
        public ExcelDataSourceDTO() : base("Excel")
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

        // TODO:  Create a field that provides a reference to the Excel
        //        data on the server

        private class ExcelDataSourceDetails
        {
            public string LocationColumn { get; set; }
            public string DateColumn { get; set; }
        }
    }

    
}
