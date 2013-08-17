using Newtonsoft.Json;

namespace Silas.Forecast.Models
{
    public class DataEntry
    {
        public int Id { get; set; }
        public int Period { get; set; }
        public double Value { get; set; }
        public int DataSetId { get; set; }

        [JsonIgnore]
        public DataSet Set { get; set; }
    }
}