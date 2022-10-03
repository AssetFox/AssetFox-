using System;
using Microsoft.Data.Sqlite;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class NetworkHistory : IDisposable
    {
        // 2 "modes", write & read. starts in write. when writing, there is no index and there's an
        // active transaction. when reading, there is an index and the transaction is committed and
        // closed. a read attempt will transition to read mode if in write mode, and vice versa.

        private readonly SqliteConnection Connection;

        private SqliteTransaction Transaction;

        // public (asset, attribute, year) => value

        // public (asset, attribute) => years

        // public (asset, attribute, year, value) => insert

        public void Dispose()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
            GC.SuppressFinalize(this);
        }

        public NetworkHistory()
        {
            SqliteConnection connection = new("Data Source=");
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                connection?.Dispose();
            };

            Connection = connection;

            // initialize SqliteCommand objects for reuse in read/write operations
        }

        private void EnsureReadMode()
        {

        }

        private void EnsureWriteMode()
        {

        }

        ~NetworkHistory() => Dispose();
    }
}
