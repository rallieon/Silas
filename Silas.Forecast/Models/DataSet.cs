using System.Collections.Generic;

namespace Silas.Forecast.Models
{
    public class DataSet
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public int CurrentPeriod { get; set; }
        public virtual ICollection<DataEntry> Entries { get; set; }
    }
}