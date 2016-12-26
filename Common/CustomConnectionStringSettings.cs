namespace Ado.Common
{
    public class CustomConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }

        public CustomConnectionStringSettings()
        {
            ProviderName = "System.Data.SqlClient";
        }
    }
}
