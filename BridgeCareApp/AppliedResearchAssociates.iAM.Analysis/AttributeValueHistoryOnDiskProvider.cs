using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace AppliedResearchAssociates.iAM.Analysis
{
    public sealed class AttributeValueHistoryOnDiskProvider : AttributeValueHistoryProvider, IDisposable
    {
        public AttributeValueHistoryOnDiskProvider()
        {
            SqliteConnection connection = new("Data Source=");
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                connection.Dispose();
            };

            Connection = connection;

            CreateTable();

            GetValueCommand = CreateGetValueCommand();
            GetYearsCommand = CreateGetYearsCommand();
            SetValueCommand = CreateSetValueCommand();
        }

        public override void ClearHistory()
        {
            YearsCache.Clear();

            EnsureReadMode();

            var command = Connection.CreateCommand();

            command.CommandText = $"DELETE FROM {TableName}";
            _ = command.ExecuteNonQuery();

            command.CommandText = "VACUUM";
            _ = command.ExecuteNonQuery();

            base.ClearHistory();
        }

        public void Dispose()
        {
            Connection?.Dispose();
            GC.SuppressFinalize(this);
        }

        protected override IAttributeValueHistory<T> CreateHistory<T>(Attribute<T> attribute) => new AttributeValueHistoryOnDisk<T>(this, attribute);

        private const string ParameterPrefix = "$";
        private const string TableName = "network_history";

        private readonly SqliteConnection Connection;

        private readonly SqliteCommand GetValueCommand;
        private readonly SqliteCommand GetYearsCommand;
        private readonly SqliteCommand SetValueCommand;

        /// <summary>
        ///     If this is not null, the history is in "write-mode"; otherwise, the history is in "read-mode".
        /// </summary>
        private SqliteTransaction Transaction
        {
            get => SetValueCommand.Transaction;
            set => SetValueCommand.Transaction = value;
        }

        ~AttributeValueHistoryOnDiskProvider() => Connection?.Dispose();

        private SqliteCommand CreateGetValueCommand()
        {
            const string commandText =
$@"SELECT {ValueColumnName}
    FROM {TableName}
    WHERE (
        {AttributeColumnName},

            {YearColumnName}
    ) = (
        {AttributeParameterName},
        {YearParameterName}
    )";

            var command = Connection.CreateCommand();

            command.CommandText = commandText;

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
    FROM
            {TableName}
    WHERE {AttributeColumnName} = {AttributeParameterName}";

            var command = Connection.CreateCommand();

            command.CommandText = commandText;

            _ = command.Parameters.Add(AttributeParameterName, AttributeType);

            return command;
        }

        private SqliteCommand CreateSetValueCommand()
        {
            const string commandText =
$@"REPLACE INTO {TableName} (
    {AttributeColumnName},
    {YearColumnName},

            {ValueColumnName}
    ) VALUES (
    {AttributeParameterName},
    {YearParameterName},
    {ValueParameterName}
    )";

            var command = Connection.CreateCommand();

            command.CommandText = commandText;

            _ = command.Parameters.Add(AttributeParameterName, AttributeType);
            _ = command.Parameters.Add(YearParameterName, YearType);
            _ = command.Parameters.Add(ValueParameterName, ValueType);

            return command;
        }

        private void CreateTable()
        {
            const string commandText =
$@"CREATE TABLE {TableName} (
    {AttributeColumnName} {AttributeTypeName} NOT NULL,
    {YearColumnName} {YearTypeName} NOT NULL,
    {ValueColumnName} {ValueTypeName} NOT NULL,
    PRIMARY KEY ( {AttributeColumnName}, {YearColumnName} )
    )";

            Connection.Open();
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

        private void EnsureWriteMode()
        {
            if (Transaction is null)
            {
                Transaction = Connection.BeginTransaction();
                YearsCache.Clear();
            }
        }

        private IReadOnlyList<KeyValuePair<int, T>> GetYears<T>(string attribute)
        {
            if (YearsCache.TryGetValue(attribute, out var _years))
            {
                return (IReadOnlyList<KeyValuePair<int, T>>)_years;
            }

            EnsureReadMode();

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

            YearsCache.Add(attribute, years);

            return years;
        }

        private readonly Dictionary<string, object> YearsCache = new();

        private void SetValue<T>(string attribute, int year, T value)
        {
            EnsureWriteMode();

            SetValueCommand.Parameters[AttributeParameterName].Value = attribute;
            SetValueCommand.Parameters[YearParameterName].Value = year;
            SetValueCommand.Parameters[ValueParameterName].Value = value;

            _ = SetValueCommand.ExecuteNonQuery();
        }

        private bool TryGetValue<T>(string attribute, int year, out T value)
        {
            EnsureReadMode();

            GetValueCommand.Parameters[AttributeParameterName].Value = attribute;
            GetValueCommand.Parameters[YearParameterName].Value = year;

            var scalar = GetValueCommand.ExecuteScalar();
            var hasValue = scalar is not null;
            value = hasValue ? (T)scalar : default;
            return hasValue;
        }

        #region Column/parameter info

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

        private sealed class AttributeValueHistoryOnDisk<T> : IAttributeValueHistory<T>
        {
            public AttributeValueHistoryOnDisk(AttributeValueHistoryOnDiskProvider provider, Attribute<T> attribute)
            {
                Provider = provider;
                Attribute = attribute;
            }

            public IEnumerable<int> Keys => this.Select(year => year.Key);

            public T MostRecentValue
            {
                get => _HasMostRecentValue ? _MostRecentValue : Attribute.DefaultValue;
                set
                {
                    _MostRecentValue = value;
                    _HasMostRecentValue = true;
                }
            }

            public T this[int year]
            {
                get => Provider.TryGetValue(Attribute.Name, year, out T value) ? value : throw new ArgumentException("Year not found in history.", nameof(year));
                set => Provider.SetValue(Attribute.Name, year, value);
            }

            public void Add(int year, T value) => this[year] = value;

            public void Add(KeyValuePair<int, T> item) => Add(item.Key, item.Value);

            public IEnumerator<KeyValuePair<int, T>> GetEnumerator() => Provider.GetYears<T>(Attribute.Name).GetEnumerator();

            public bool TryGetValue(int key, out T value) => Provider.TryGetValue(Attribute.Name, key, out value);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private readonly Attribute<T> Attribute;
            private readonly AttributeValueHistoryOnDiskProvider Provider;

            private bool _HasMostRecentValue;
            private T _MostRecentValue;
        }
    }
}
