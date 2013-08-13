using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silas.Forecast.Models
{
    public class DataSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CurrentPeriod { get; set; }
        public virtual ICollection<DataEntry> Entries { get; set; }
    }
}
