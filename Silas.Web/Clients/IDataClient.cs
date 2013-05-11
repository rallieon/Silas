using System.Collections.Generic;
using Silas.Domain;

namespace Silas.Web.Clients
{
    internal interface IDataClient
    {
        IEnumerable<DataEntry> GetData();
        DataEntry GetLatestEntry();
    }
}