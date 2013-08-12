using System.Data.Entity;
using Silas.Forecast.Models;

namespace Silas.Server.DB
{
    public class ForecastContext : DbContext
    {
        public ForecastContext()
            : base("LiveDataConnection")
        {
        }

        public DbSet<DataEntry> Entries { get; set; }
    }
}