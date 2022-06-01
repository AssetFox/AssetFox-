using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public static class RowBasedExcelRegionModels
    {
        public static RowBasedExcelRegionModel Concat(
            List<RowBasedExcelRegionModel> regions)
        {
            var rows = new List<ExcelRowModel>();
            foreach (var region in regions)
            {
                rows.AddRange(region.Rows);
            }
            return new RowBasedExcelRegionModel
            {
                Rows = rows,
            };
        }

        public static RowBasedExcelRegionModel Column(List<IExcelModel> content) 
        {
            var rows = content.Select(x => ExcelRowModels.WithEntries(x)).ToList();
            return WithRows(rows);
        }

        public static RowBasedExcelRegionModel Concat(
            params RowBasedExcelRegionModel[] regions)
            => Concat(regions.ToList());

        public static RowBasedExcelRegionModel WithRows(List<ExcelRowModel> rows)
            => new RowBasedExcelRegionModel
            {
                Rows = rows,
            };

        public static RowBasedExcelRegionModel WithRows(params ExcelRowModel[] rows)
            => WithRows(rows.ToList());

        public static RowBasedExcelRegionModel BlankLine =>
            WithRows(ExcelRowModels.Empty);
        
    }
}
