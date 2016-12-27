using Ado.Common;
using Ado.Enumerators;
using Ado.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Ado
{
    public class DataContext : IDisposable
    {
        static DataContext()
        {
            DefaultConnectionString = "DefaultConnection";
        }
        public DataContext()
            : this(null, Transaction.Begin) { }

        public DataContext(string connectionString)
            : this(connectionString, Transaction.Begin) { }

        public DataContext(Transaction transactionMode)
            : this(null, transactionMode) { }

        public DataContext(string connectionString, Transaction transaction)
        {
            TransactionMode = transaction;

            var connectionName = connectionString ?? DefaultConnectionString;
            ConnectionSettings = ConnectionStringBuffer.Instance.Get(connectionName);

            Provider = new ProviderFactory().Create(ConnectionSettings, transaction);
        }

        public IDataProvider Provider { get; private set; }
        public static string DefaultConnectionString { get; set; }
        public CustomConnectionStringSettings ConnectionSettings { get; private set; }
        public Transaction TransactionMode { get; private set; }
        public IDataReader Query(string query, params object[] queryParams)
        {
            return Provider.Query(query, queryParams);
        }
        public int ExecuteCommand(string command, bool commit = true, params object[] commandParams)
        {
            int rowsAffects = 0;
            try
            {
                rowsAffects = Provider.Execute(command, commandParams);
                if (commit)
                    Commit();
            }
            catch (Exception)
            {
                RollBack();
                Dispose();
            }
            return rowsAffects;
        }
        public int ExecuteGetIdentity(string command, params object[] commandParams)
        {
            return Provider.ExecuteGetIdentity(command, commandParams);
        }
        public object GetValue(string query, params object[] queryParams)
        {
            return Provider.ExecuteGetValue(query, queryParams);
        }
        public T GetValue<T>(string query, params object[] queryParams)
        {
            var result = GetValue(query, queryParams);
            return DataReader.CastTo<T>(result);
        }
        public IEnumerable<T> GetValues<T>(string query, params object[] queryParams)
        {
            var data = Query(query, queryParams);
            return new DataReader(data).ToEnumerable<T>();
        }
        public IEnumerable<T> GetWhere<T>(string query, params object[] queryParams) where T : new()
        {
            var data = Query(query, queryParams);
            return new DataReader(data).ToObjectList<T>();
        }
        public void Commit()
        {
            Provider.Commit();
        }
        public void Dispose()
        {
            Provider.Dispose();
        }
        public void RollBack()
        {
            Provider.RollBack();
        }
    }
}
