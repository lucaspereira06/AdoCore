using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using Ado.Common;

namespace Ado
{
    public class ConnectionStringBuffer
    {
        #region Definição Singleton

        private static readonly ConnectionStringBuffer _instance;
        static ConnectionStringBuffer()
        {
            _instance = new ConnectionStringBuffer();
        }

        public static ConnectionStringBuffer Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Definições Class

        private readonly Dictionary<string, CustomConnectionStringSettings> _buffer;

        public ConnectionStringBuffer()
        {
            _buffer = new Dictionary<string, CustomConnectionStringSettings>();
        }

        public CustomConnectionStringSettings Get(string connectionStringName)
        {
            if (!_buffer.ContainsKey(connectionStringName))
            {
                _buffer.Add(connectionStringName, GetFromConfig(connectionStringName));
            }

            return _buffer[connectionStringName];
        }

        private CustomConnectionStringSettings GetFromConfig(string connectionName)
        {
            var setting =
                new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build().GetConnectionString(connectionName);

            if (setting == null)
            {
                var exceptionMessage = string.Format("ConnectionString :{0} não encontrada.", connectionName);
                throw new Exception(exceptionMessage);
            }

            return new CustomConnectionStringSettings { ConnectionString = setting };
        }

        #endregion
    }
}
