using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class DataSourceEntity: BaseEntity
    {
        public DataSourceEntity()
        {
            ExcelWorksheets = new HashSet<ExcelWorksheetEntity>();
        }
        public Guid Id { get; set; }

        /// <summary>
        /// Name to be displayed by the UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of data source, provided by the specific instance of the data source
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Indicates if the details should be obscured in the database
        /// </summary>
        /// <example>
        /// A connection string that contains passwords should be secured
        /// </example>
        public bool Secure { get; set; }

        /// <summary>
        /// JSON formatted string containing the implementation details of the data source
        /// </summary>
        /// <example>
        /// The details for a SQL data source would be the connection string
        /// </example>
        public string Details { get; set; }

        public virtual ICollection<ExcelWorksheetEntity> ExcelWorksheets { get; set; }

    }
}
