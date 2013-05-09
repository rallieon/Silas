using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silas.Domain;

namespace Silas.Server.DB
{
    public class TrueDataContext : DbContext
    {
        public TrueDataContext()
            : base("TrueDataConnection") { }

        public DbSet<DataEntry> Entries { get; set; }
    }
}
