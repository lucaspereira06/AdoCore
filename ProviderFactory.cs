using Ado.Common;
using Ado.Enumeradores;
using Ado.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Ado
{
    public class ProviderFactory
    {
        public static Func<string, IDataProvider> CustomProvider { get; set; }
        public static Func<string, IDbConnection> ConnectionFactory { get; set; }

        private static readonly Dictionary<string, Type> Providers;

        static ProviderFactory()
        {
            Providers = new Dictionary<string, Type>();
            AddProvider("System.Data.SqlClient", typeof(SqlProvider));
        }

        public IDataProvider Create(CustomConnectionStringSettings settings, Transaction transactionMode)
        {
            var provider = ResolveDataProvider(settings.ProviderName);
            provider.DbConnection = CreateConnection(settings.ProviderName, settings);
            provider.TransactionMode = transactionMode;
            return provider;
        }

        public IDataProvider ResolveDataProvider(string providerName)
        {
            if (CustomProvider != null) return CustomProvider(providerName);

            if (Providers.ContainsKey(providerName))
                return (IDataProvider)Activator.CreateInstance(Providers[providerName]);

            throw new Exception(string.Format("Provider não suportado : {0}", providerName));
        }

        private IDbConnection CreateConnection(string providerName, CustomConnectionStringSettings settings)
        {
            IDbConnection connection;

            if (ConnectionFactory != null)
                connection = ConnectionFactory(providerName);
            else
                connection = DbProviderFactories.GetFactory(providerName).CreateConnection();

            connection.ConnectionString = settings.ConnectionString;

            return connection;
        }

        public static void AddProvider(string providerName, Type providerType)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException("Parametro 'providerName' não pode ser nulo.");

            if (providerType == null)
                throw new ArgumentNullException("providerType");

            if (Providers.ContainsKey(providerName))
            {
                if (!Providers[providerName].Equals(providerType))
                {
                    var message = string.Format("Attempting to add a provider named '{0}' of type '{1}', but an existing provider of type '{2}' already exists with that name.",
                        providerName, providerType.ToString(), Providers[providerName].ToString());

                    throw new Exception(message);
                }
            }
            else
                Providers.Add(providerName, providerType);
        }
    }
}
