using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Web;
using DevExpress.DataAccess.Wizard.Services;
using Microsoft.Extensions.Configuration;
using PowerAPI.Data.Models;

namespace PowerAPI.Service.Helper
{
    public class CustomSqlDataConnectionProviderFactory : IConnectionProviderFactory
    {
        readonly IConnectionProviderService connectionProviderService;

        public CustomSqlDataConnectionProviderFactory(IConnectionProviderService connectionProviderService)
        {
            this.connectionProviderService = connectionProviderService;
        }

        public IConnectionProviderService Create()
        {
            return connectionProviderService;
        }
    }

    public class CustomSqlConnectionProviderService : IConnectionProviderService
    {
        readonly IConfiguration configuration;

        public CustomSqlConnectionProviderService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public SqlDataConnection LoadConnection(string connectionName)
        {
            var connectionStringSection = configuration.GetSection("ReportingDataConnectionStrings");
            var connectionString = connectionStringSection?[connectionName];

            if (string.IsNullOrEmpty(connectionString))
                throw new KeyNotFoundException($"Connection string '{connectionName}' not found.");

            var connectionParameters = new CustomStringConnectionParameters(connectionString);
            return new SqlDataConnection(connectionName, connectionParameters);
        }
    }
}