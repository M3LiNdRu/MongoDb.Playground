using System;
using Microsoft.Extensions.Options;
using MongoDb.Playground.Repository.Configuration;

namespace MongoDbPayground
{
    internal class OptionsMonitorDbConfiguration : IOptionsMonitor<DbConfigurationSettings>
    {
        private readonly DbConfigurationSettings _config;

        public OptionsMonitorDbConfiguration(string connectionString, string database)
        {
            _config = new DbConfigurationSettings
            {
                ConnectionString = connectionString,
                Database = database
            };
        }

        public DbConfigurationSettings CurrentValue => _config;

        public DbConfigurationSettings Get(string name)
        {
            return _config;
        }

        public IDisposable OnChange(Action<DbConfigurationSettings, string> listener)
        {
            throw new NotImplementedException();
        }
    }
}