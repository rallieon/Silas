using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;

namespace Silas.API.Models
{
    public class DataEntry
    {
        [CsvField(Ignore = true)]
        public int Id { get; set; }

        [CsvField(Index = 0)]
        public DateTime DateTime { get; set; }

        [CsvField(Index = 1)]
        public int Value { get; set; }
    }

    public class DataEntryContext : DbContext
    {
        public DataEntryContext()
            : base("DataEntriesConnection"){ }

        public DbSet<DataEntry> Entries { get; set; }
    }

    public class DataEntryEmptyContext : DbContext
    {
        public DataEntryEmptyContext()
            : base("DataEntriesEmptyConnection") { }

        public DbSet<DataEntry> Entries { get; set; }
    }
}