using System;
using System.Collections.Generic;
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

        public IReadOnlyList<KeyValuePair<int, T>> GetYears<T>(string asset, string attribute)
        {
            lock (Lock)
            {
                EnsureReadMode();

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
        }

        public void SetValue<T>(string asset, string attribute, int year, T value)
        {
            lock (Lock)
            {
                EnsureWriteMode();

                SetValueCommand.Parameters[AssetParameterName].Value = asset;
                SetValueCommand.Parameters[AttributeParameterName].Value = attribute;
                SetValueCommand.Parameters[YearParameterName].Value = year;
                SetValueCommand.Parameters[ValueParameterName].Value = value;

                _ = SetValueCommand.ExecuteNonQuery();
            }
        }

        public bool TryGetValue<T>(string asset, string attribute, int year, out T value)
        {
            lock (Lock)
            {
                EnsureReadMode();

                GetValueCommand.Parameters[AssetParameterName].Value = asset;
                GetValueCommand.Parameters[AttributeParameterName].Value = attribute;
                GetValueCommand.Parameters[YearParameterName].Value = year;

                var scalar = GetValueCommand.ExecuteScalar();
                var hasValue = scalar is not null;
                value = hasValue ? (T)scalar : default;
                return hasValue;
            }
        }

        #region "asset" metadata

        private const string AssetColumnName = "asset";
        private const string AssetParameterName = ParameterPrefix + AssetColumnName;
        private const SqliteType AssetType = SqliteType.Text;
        private const string AssetTypeName = "TEXT";

        #endregion "asset" metadata

        #region "attribute" metadata

        private const string AttributeColumnName = "attribute";
        private const string AttributeParameterName = ParameterPrefix + AttributeColumnName;
        private const SqliteType AttributeType = SqliteType.Text;
        private const string AttributeTypeName = "TEXT";

        #endregion "attribute" metadata

        #region "year" metadata

        private const string YearColumnName = "year";
        private const string YearParameterName = ParameterPrefix + YearColumnName;
        private const SqliteType YearType = SqliteType.Integer;
        private const string YearTypeName = "INTEGER";

        #endregion "year" metadata

        #region "value" metadata

        private const string ValueColumnName = "value";
        private const string ValueParameterName = ParameterPrefix + ValueColumnName;
        private const SqliteType ValueType = SqliteType.Blob;
        private const string ValueTypeName = "BLOB";

        #endregion "value" metadata

        private const string ParameterPrefix = "$";
        private const string TableName = "network_history";

        private readonly SqliteConnection Connection;
        private readonly SqliteCommand GetValueCommand;
        private readonly SqliteCommand GetYearsCommand;
        private readonly object Lock = new();
        private readonly SqliteCommand SetValueCommand;

        /// <summary>
        ///     If this is null, the history is in "read mode"; otherwise, the history is in "write mode".
        /// </summary>
        private SqliteTransaction Transaction
        {
            get => SetValueCommand.Transaction;
            set => SetValueCommand.Transaction = value;
        }

        ~NetworkHistory()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
        }

        private void CreateTable()
        {
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
            if (Transaction is not null)
            {
                Transaction.Commit();
                Transaction.Dispose();
                Transaction = null;
            }
        }

        private void EnsureWriteMode() => Transaction ??= Connection.BeginTransaction();

        private void Initialize()
        {
            InitializeGetValueCommand();
            InitializeGetYearsCommand();
            InitializeSetValueCommand();

            Connection.Open();

            CreateTable();

            Transaction = Connection.BeginTransaction();
        }

        private void InitializeGetValueCommand()
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

            var command = GetValueCommand;

            command.CommandText = commandText;

            _ = command.Parameters.Add(AssetParameterName, AssetType);
            _ = command.Parameters.Add(AttributeParameterName, AttributeType);
            _ = command.Parameters.Add(YearParameterName, YearType);
        }

        private void InitializeGetYearsCommand()
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

            var command = GetYearsCommand;

            command.CommandText = commandText;

            _ = command.Parameters.Add(AssetParameterName, AssetType);
            _ = command.Parameters.Add(AttributeParameterName, AttributeType);
        }

        private void InitializeSetValueCommand()
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

            var command = SetValueCommand;

            command.CommandText = commandText;

            _ = command.Parameters.Add(AssetParameterName, AssetType);
            _ = command.Parameters.Add(AttributeParameterName, AttributeType);
            _ = command.Parameters.Add(YearParameterName, YearType);
            _ = command.Parameters.Add(ValueParameterName, ValueType);
        }
    }
}
