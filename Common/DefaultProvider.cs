using Ado.Enumerators;
using Ado.Interfaces;
using System;
using System.Data;
using System.Data.Common;

namespace Ado.Common
{
    public abstract class DefaultProvider : IDataProvider
    {
        public IDbConnection DbConnection { get; set; }
        public IDbTransaction DbTransaction { get; set; }
        public abstract string FieldFormat { get; }
        public abstract string ParameterIdentifier { get; }
        public Transaction TransactionMode { get; set; }

        public void Commit()
        {
            if (DbTransaction != null) DbTransaction.Commit();
        }

        public void CreateConnection(CustomConnectionStringSettings connectionSettings, Transaction transaction)
        {
            TransactionMode = transaction;
            DbConnection = DbProviderFactories.GetFactory(connectionSettings.ProviderName).CreateConnection();
            DbConnection.ConnectionString = connectionSettings.ConnectionString;
        }

        public void Dispose()
        {
            if (DbTransaction != null) DbTransaction.Dispose();
            if (DbConnection != null) DbConnection.Close();
        }

        public int Execute(string command, object[] commandParams)
        {
            return CreateDbCommand(command, commandParams).ExecuteNonQuery();
        }

        public abstract int ExecuteGetIdentity(string command, object[] commandParams);

        public object ExecuteGetValue(string query, object[] queryParams)
        {
            return CreateDbCommand(query, queryParams, transactional: false).ExecuteScalar();
        }

        public IDataReader Query(string query, object[] queryParams)
        {
            return CreateDbCommand(query, queryParams, transactional: false).ExecuteReader();
        }

        public void RollBack()
        {
            if (DbTransaction != null) DbTransaction.Rollback();
        }

        protected IDbCommand CreateDbCommand(string query, object[] objectParameters, bool transactional = true)
        {
            IDbCommand command = null;
            try
            {
                Open(transactional);

                command = DbConnection.CreateCommand();
                command.CommandText = query;
                command.Connection = DbConnection;
                command.Transaction = DbTransaction;
                if (objectParameters != null)
                    new ParametersBinder(ParameterIdentifier, objectParameters).Bind(command);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return command;
        }

        private void Open(bool transactional)
        {
            if (DbConnection.State == ConnectionState.Closed)
                DbConnection.Open();

            if (TransactionMode == Transaction.Begin && transactional && DbTransaction == null)
                DbTransaction = DbConnection.BeginTransaction();
        }
    }
}
