using CsvHelper.Configuration;
using Silas.Forecast.Models;

namespace Silas.Client
{
    class DataEntryMap : CsvClassMap<DataEntry>
    {
        public DataEntryMap()
        {
            Map(m => m.Period).Index(0);
            Map(m => m.Value).Index(1);
            Map(m => m.Id).Ignore();
        }
    }
}
