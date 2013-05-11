using System;
using CsvHelper.Configuration;

namespace Silas.Domain
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
}