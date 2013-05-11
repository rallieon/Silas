﻿using System.Data.Entity;
using Silas.Domain;

namespace Silas.Server.DB
{
    public class LiveDataContext : DbContext
    {
        public LiveDataContext()
            : base("LiveDataConnection")
        {
        }

        public DbSet<DataEntry> Entries { get; set; }
    }
}