using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class NetworkHistoryOnDisk : INetworkHistory, IDisposable
    {
        public NetworkHistoryOnDisk()
        {
            SqliteConnection connection = new("Data Source=");
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                connection.Dispose();
            };

            Connection = connection;

            CreateTable();

            Transaction = Connection.BeginTransaction();
        }

        public void Dispose()
        {
            Connection?.Dispose();
            GC.SuppressFinalize(this);
        }

        public INetworkHistoryAccessor GetAccessor() => new Accessor(this);

        private const string ParameterPrefix = "$";
        private const string TableName = "network_history";

        private readonly SqliteConnection Connection;
        private readonly object TransactionLock = new();

        /// <summary>
        ///     If this is not null, the history is in "write-mode"; otherwise, the history is in
        ///     "read-mode". The history starts in write-mode and transitions automatically and
        ///     permanently to read-mode as soon as any read-operation occurs.
        /// </summary>
        private SqliteTransaction Transaction;

        #region Column/parameter info

        private const string AssetColumnName = "asset";
        private const string AssetParameterName = ParameterPrefix + AssetColumnName;
        private const SqliteType AssetType = SqliteType.Text;
        private const string AssetTypeName = "TEXT";

        private const string AttributeColumnName = "attribute";
        private const string AttributeParameterName = ParameterPrefix + AttributeColumnName;
        private const SqliteType AttributeType = SqliteType.Text;
        private const string AttributeTypeName = "TEXT";

        private const string ValueColumnName = "value";
        private const string ValueParameterName = ParameterPrefix + ValueColumnName;
        private const SqliteType ValueType = SqliteType.Blob;
        private const string ValueTypeName = "BLOB";

        private const string YearColumnName = "year";
        private const string YearParameterName = ParameterPrefix + YearColumnName;
        private const SqliteType YearType = SqliteType.Integer;
        private const string YearTypeName = "INTEGER";

        #endregion Column/parameter info

        ~NetworkHistoryOnDisk() => Connection?.Dispose();

        private void CreateTable()
        {
            Connection.Open();

            const string commandText =
$@"CREATE TABLE {TableName} (
    {AssetColumnName} {AssetTypeName} NOT NULL,
    {AttributeColumnName} {AttributeTypeName} NOT NULL,
    {YearColumnName} {YearTypeName} NOT NULL,
    {ValueColumnName} {ValueTypeName} NOT NULL,
    PRIMARY KEY ( {AssetColumnName}, {AttributeColumnName}, {YearColumnName} )
    )";

            var command = Connection.CreateCommand();
            command.CommandText = commandText;
            _ = command.ExecuteNonQuery();
        }

        private void EnsureReadMode()
        {
            lock (TransactionLock)
            {
                if (Transaction is not null)
                {
                    Transaction.Commit();
                    Transaction.Dispose();
                    Transaction = null;
                }
            }
        }

        private void ValidateWriteMode()
        {
            if (Transaction is null)
            {
                throw new InvalidOperationException("Transaction is closed.");
            }
        }

        private sealed class Accessor : INetworkHistoryAccessor
        {
            public Accessor(NetworkHistoryOnDisk history)
            {
                History = history;

                _GetValueCommand = new(CreateGetValueCommand);
                _GetYearsCommand = new(CreateGetYearsCommand);
                _SetValueCommand = new(CreateSetValueCommand);
            }

            public void Clear()
            {
                History.EnsureReadMode();

                var command = History.Connection.CreateCommand();

                command.CommandText = $"DELETE FROM {TableName}";
                _ = command.ExecuteNonQuery();

                command.CommandText = "VACUUM";
                _ = command.ExecuteNonQuery();
            }

            public IReadOnlyList<KeyValuePair<int, T>> GetYears<T>(string asset, string attribute)
            {
                History.EnsureReadMode();

                GetYearsCommand.Parameters[AssetParameterName].Value = asset;
                GetYearsCommand.Parameters[AttributeParameterName].Value = attribute;

                using var reader = GetYearsCommand.ExecuteReader();
                var yearColumnOrdinal = reader.GetOrdinal(YearColumnName);
                var valueColumnOrdinal = reader.GetOrdinal(ValueColumnName);

                List<KeyValuePair<int, T>> years = new();
                while (reader.Read())
                {
                    var year = reader.GetInt32(yearColumnOrdinal);
                    var value = reader.GetFieldValue<T>(valueColumnOrdinal);

                    years.Add(new(year, value));
                }

                return years;
            }

            public void SetValue<T>(string asset, string attribute, int year, T value)
            {
                History.ValidateWriteMode();

                SetValueCommand.Parameters[AssetParameterName].Value = asset;
                SetValueCommand.Parameters[AttributeParameterName].Value = attribute;
                SetValueCommand.Parameters[YearParameterName].Value = year;
                SetValueCommand.Parameters[ValueParameterName].Value = value;

                _ = SetValueCommand.ExecuteNonQuery();
            }

            public bool TryGetValue<T>(string asset, string attribute, int year, out T value)
            {
                History.EnsureReadMode();

                GetValueCommand.Parameters[AssetParameterName].Value = asset;
                GetValueCommand.Parameters[AttributeParameterName].Value = attribute;
                GetValueCommand.Parameters[YearParameterName].Value = year;

                var scalar = GetValueCommand.ExecuteScalar();
                var hasValue = scalar is not null;
                value = hasValue ? (T)scalar : default;
                return hasValue;
            }

            private readonly Lazy<SqliteCommand> _GetValueCommand;
            private readonly Lazy<SqliteCommand> _GetYearsCommand;
            private readonly Lazy<SqliteCommand> _SetValueCommand;

            private readonly NetworkHistoryOnDisk History;

            private SqliteCommand GetValueCommand => _GetValueCommand.Value;

            private SqliteCommand GetYearsCommand => _GetYearsCommand.Value;

            private SqliteCommand SetValueCommand => _SetValueCommand.Value;

            private SqliteCommand CreateGetValueCommand()
            {
                const string commandText =
$@"SELECT {ValueColumnName}
    FROM {TableName}
    WHERE (
        {AssetColumnName},
        {AttributeColumnName},
        {YearColumnName}
    ) = (
        {AssetParameterName},
        {AttributeParameterName},
        {YearParameterName}
    )";

                var command = History.Connection.CreateCommand();

                command.CommandText = commandText;

                _ = command.Parameters.Add(AssetParameterName, AssetType);
                _ = command.Parameters.Add(AttributeParameterName, AttributeType);
                _ = command.Parameters.Add(YearParameterName, YearType);

                return command;
            }

            private SqliteCommand CreateGetYearsCommand()
            {
                const string commandText =
$@"SELECT
    {YearColumnName},
    {ValueColumnName}
    FROM {TableName}
    WHERE (
        {AssetColumnName},
        {AttributeColumnName}
    ) = (
        {AssetParameterName},
        {AttributeParameterName}
    )
    ORDER BY {YearColumnName} ASC";

                var command = History.Connection.CreateCommand();

                command.CommandText = commandText;

                _ = command.Parameters.Add(AssetParameterName, AssetType);
                _ = command.Parameters.Add(AttributeParameterName, AttributeType);

                return command;
            }

            private SqliteCommand CreateSetValueCommand()
            {
                const string commandText =
$@"REPLACE INTO {TableName} (
    {AssetColumnName},
    {AttributeColumnName},
    {YearColumnName},
    {ValueColumnName}
    ) VALUES (
    {AssetParameterName},
    {AttributeParameterName},
    {YearParameterName},
    {ValueParameterName}
    )";

                var command = History.Connection.CreateCommand();

                command.CommandText = commandText;

                _ = command.Parameters.Add(AssetParameterName, AssetType);
                _ = command.Parameters.Add(AttributeParameterName, AttributeType);
                _ = command.Parameters.Add(YearParameterName, YearType);
                _ = command.Parameters.Add(ValueParameterName, ValueType);

                return command;
            }
        }
    }
}
