using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions
{
    public static class IListExtensions
    {
        public static IEnumerable<List<T>> ConcreteBatch<T>(this IList<T> list, int batchSize)
        {
            int startIndex = 0;
            int batchIndex = 0;
            while (startIndex < list.Count)
            {
                int endIndex = Math.Min(startIndex + batchSize, list.Count);
                var batch = list.Skip(startIndex).Take(endIndex - startIndex).ToList();
                yield return batch;
                batchIndex++;
                startIndex = batchIndex * batchSize;
            }
        }
    }
}
