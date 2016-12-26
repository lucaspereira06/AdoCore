using System.Collections.Generic;
using System.Data.SqlClient;

namespace System.Data.Common
{
    public abstract class DbProviderFactories
    {
        internal static readonly Dictionary<string, Func<DbProviderFactory>> _configs = new Dictionary<string, Func<DbProviderFactory>>();

        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            if (_configs.ContainsKey(providerInvariantName))
            {
                return _configs[providerInvariantName]();
            }

            throw new Exception("Configuration Provider not found.");
        }

        public static void Add(string key, Func<DbProviderFactory> constructorDelegate)
        {

            if (_configs.ContainsKey(key))
            {
                throw new Exception("Configuration Provider Key Already Exists.");
            }

            _configs.Add(key, constructorDelegate);
        }

        public static void AddSqlServerFactory()
        {
            var key = "System.Data.SqlClient";

            if (!_configs.ContainsKey(key))
            {
                _configs.Add(key, SqlFactory);
            }
        }

        private static SqlClientFactory SqlFactory()
        {
            return SqlClientFactory.Instance;
        }
    }
}
