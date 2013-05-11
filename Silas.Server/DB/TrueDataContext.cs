using System.Data.Entity;
using Silas.Domain;

namespace Silas.Server.DB
{
    public class TrueDataContext : DbContext
    {
        public TrueDataContext()
            : base("TrueDataConnection")
        {
        }

        public DbSet<DataEntry> Entries { get; set; }
    }
}