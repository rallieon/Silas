using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Silas.Domain;

namespace Silas.Server.DB
{
    public class LiveDataContext : DbContext
    {
        public LiveDataContext()
            : base("LiveDataConnection") { }

        public DbSet<DataEntry> Entries { get; set; }
    }
}