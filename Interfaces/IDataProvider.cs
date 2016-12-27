using Ado.Common;
using Ado.Enumerators;
using System;
using System.Data;

namespace Ado.Interfaces
{
    public interface IDataProvider : IDisposable
    {
        IDbConnection DbConnection { get; set; }
        IDbTransaction DbTransaction { get; set; }
        string ParameterIdentifier { get; }
        string FieldFormat { get; }
        Transaction TransactionMode { get; set; }
        IDataReader Query(string query, object[] queryParams);
        int Execute(string command, object[] commandParams);
        int ExecuteGetIdentity(string command, object[] commandParams);
        object ExecuteGetValue(string query, object[] queryParams);
        void CreateConnection(CustomConnectionStringSettings settings, Transaction transaction);
        void Commit();
        void RollBack();
    }
}
