using System;
using Microsoft.Data.Sqlite;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class NetworkHistory : IDisposable
    {
        public NetworkHistory()
        {
            SqliteConnection connection = new("Data Source=");
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                connection?.Dispose();
            };

            GetValueCommand = connection.CreateCommand();
            GetYearsCommand = connection.CreateCommand();
            SetValueCommand = connection.CreateCommand();

            Connection = connection;

            Initialize();
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
            GC.SuppressFinalize(this);
        }

        public T GetValue<T>(string asset, string attribute, int year)
        {
            EnsureReadMode();
        }

        public int[] GetYears(string asset, string attribute)
        {
            EnsureReadMode();
        }

        public void SetValue<T>(string asset, string attribute, int year, T value)
        {
            EnsureWriteMode();
        }

        private readonly SqliteConnection Connection;
        private readonly SqliteCommand GetValueCommand;
        private readonly SqliteCommand GetYearsCommand;
        private readonly object ModeLock = new();
        private readonly SqliteCommand SetValueCommand;
        private SqliteTransaction Transaction;

        ~NetworkHistory()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
        }

        private void EnsureReadMode()
        {
            lock (ModeLock)
            {
                if (Transaction is not null)
                {
                    Transaction.Commit();
                    Transaction.Dispose();
                    Transaction = null;
                }
            }
        }

        private void EnsureWriteMode()
        {
            lock (ModeLock)
            {
                Transaction ??= Connection.BeginTransaction();
            }
        }

        private void Initialize()
        {
            GetValueCommand.CommandText = "INSERT INTO ";

            GetYearsCommand.CommandText = "INSERT INTO ";

            SetValueCommand.CommandText = "REPLACE INTO ";

            Connection.Open();

            var createTableCommand = Connection.CreateCommand();
            createTableCommand.CommandText =
@"CREATE TABLE network_history (
    asset TEXT,
    attribute TEXT,
    year INTEGER,
    value,
    PRIMARY KEY ( asset, attribute, year )
)";

            _ = createTableCommand.ExecuteNonQuery();

            Transaction = Connection.BeginTransaction();
        }
    }
}
