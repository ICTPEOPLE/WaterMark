using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocMng_Api.Configuration
{
    public class Configuration : IConfiguration
    {
        private IConfigurationRoot _configuration;
        public Configuration()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddJsonFile("AppSettings.json");
            _configuration = configurationBuilder.Build();
        }

        public string FileServerPath => _configuration["AppSetting:FileServerPath"];

        public string SqliteConnectionString => _configuration["AppSetting:SQLiteConnection:ConnectionString"];

        public bool ServerCertificateValidation => Convert.ToBoolean(_configuration["AppSetting:ServerCertificateValidation"]);
    }
}
