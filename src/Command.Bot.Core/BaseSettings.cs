using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Command.Bot.Core
{
    public class BaseSettings
    {
        private readonly string _configGroup;
        private readonly IConfiguration _configuration;
        protected static readonly Lazy<IConfiguration> _config = new Lazy<IConfiguration>(ReadConfig);

        public BaseSettings(IConfiguration configuration, string configGroup)
        {
            _configuration = configuration;
            _configGroup = configGroup;
        }

        public static IConfiguration Config => _config.Value;

        #region Private Methods

        protected static IConfiguration ReadConfig()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development";
            Log.Debug($"Environment: '{environment}'");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environment}.json", true, true)
                .AddEnvironmentVariables();
            return config.Build();
        }

        #endregion

        protected string ReadConfigValue(string key, string defaultValue)
        {
            var section = string.IsNullOrWhiteSpace(_configGroup)
                ? _configuration
                : _configuration.GetSection(_configGroup);
            var value = section[key];
            return value ?? defaultValue;
        }

        protected void WriteConfigValue(string key, string value)
        {
            var section = string.IsNullOrWhiteSpace(_configGroup)
                ? _configuration
                : _configuration.GetSection(_configGroup);
            section[key] = value;
        }
    }
}