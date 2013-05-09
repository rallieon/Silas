using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silas.Domain;

namespace Silas.Web.Clients
{
    interface IDataClient
    {
        IEnumerable<DataEntry> GetData()
    }
}
