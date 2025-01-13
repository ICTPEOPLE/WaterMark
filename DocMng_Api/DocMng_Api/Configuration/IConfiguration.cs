using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocMng_Api.Configuration
{
    public interface IConfiguration
    {
        string FileServerPath { get; }

        string SqliteConnectionString { get; }

        bool ServerCertificateValidation { get; }
    }
}
