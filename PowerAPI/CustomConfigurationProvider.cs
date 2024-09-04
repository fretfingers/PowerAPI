using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;


namespace PowerAPI
{
    public class CustomConfigurationProvider
    {
        readonly IWebHostEnvironment hostingEnvironment;
   
        public CustomConfigurationProvider(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public IDictionary<string, string> GetGlobalConnectionStrings(string defaultConnection)
        {
            var connectionStrings = new Dictionary<string, string>
            {
                [$"ReportingDataConnectionStrings:Enterprise"] = defaultConnection
                //[$"ReportingDataConnectionStrings:Enterprise"] = "Server=.;Database=EnterprisePE;User Id=enterprise;Password=entx!2003n;Trusted_Connection=False;TrustServerCertificate=True;MultipleActiveResultSets=true"
                //[$"ReportingDataConnectionStrings:Enterprise"] = "XpoProvider=MSSqlServer;Data Source=.;Initial Catalog=EnterprisePE;User ID=enterprise;Password=entx!2003n;Trusted_Connection=False;TrustServerCertificate=True;MultipleActiveResultSets=true;=SQLite;Data Source=Data/cars.db;"
            };


            return new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true)
                .AddInMemoryCollection(connectionStrings)
                .AddEnvironmentVariables()
                .Build()
                .GetSection("ReportingDataConnectionStrings")
                .AsEnumerable(true)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public IConfigurationSection GetReportDesignerWizardConfigurationSection()
        {
            return new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build()
                .GetSection("ReportingDataConnectionStrings");
        }

    }
}
