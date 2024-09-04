using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PowerAPI.Data.Models
{
    public class SqlDataConnectionDescription : DataConnection { }
    public class JsonDataConnectionDescription : DataConnection { }
    public abstract class DataConnection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ReportItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public byte[] LayoutData { get; set; }
    }

    public class ReportDbContext : DbContext
    {
        public DbSet<JsonDataConnectionDescription> JsonDataConnections { get; set; }
        public DbSet<SqlDataConnectionDescription> SqlDataConnectionsDx { get; set; }
        public DbSet<ReportItem> Reports { get; set; }
        public IConfiguration Configuration { get; }
        public ReportDbContext(DbContextOptions<ReportDbContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }
        public void InitializeDatabase()
        {
            Database.EnsureCreated();

           
            var reportsDataConnectionName = "ReportsDataSqlServer";
            if (!SqlDataConnectionsDx.Any(x => x.Name == reportsDataConnectionName))
            {
                var newData = new SqlDataConnectionDescription
                {
                    Name = reportsDataConnectionName,
                    DisplayName = "Reports Data",
                    ConnectionString = Configuration.GetConnectionString("Enterprise")
                    //ConnectionString = "XpoProvider=SQLite;Data Source=|DataDirectory|/Data/reportsData.db"
                };
                SqlDataConnectionsDx.Add(newData);
            }
            SaveChanges();
        }
    }
}