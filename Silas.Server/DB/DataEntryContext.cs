using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Silas.Domain;

namespace Silas.Server.DB
{
    public class DataEntryContext : DbContext
    {
        public DataEntryContext()
            : base("DataEntriesConnection") { }

        public DbSet<DataEntry> Entries { get; set; }
    }

    public class DataEntryEmptyContext : DbContext
    {
        public DataEntryEmptyContext()
            : base("DataEntriesEmptyConnection") { }

        public DbSet<DataEntry> Entries { get; set; }
    }
}