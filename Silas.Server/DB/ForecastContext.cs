using System.Data.Entity;
using Silas.Forecast.Models;

namespace Silas.Server.DB
{
    public class ForecastContext : DbContext
    {
        public ForecastContext()
            : base("ForecastDataConnection")
        {
        }

        public DbSet<DataEntry> DataEntries { get; set; }
        public DbSet<DataSet> DataSets { get; set; }
    }
}