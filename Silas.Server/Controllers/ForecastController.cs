using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Repositories.Interfaces;
using Silas.Forecast.Models;
using Silas.Server.DB;

namespace Silas.API.Controllers
{
    public class ForecastController : ApiController
    {
        private readonly IRepository _repository;

        public ForecastController(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<DataEntry> Get()
        {
            return _repository.Get<DataEntry>();
        }

        public DataEntry Get(int period)
        {
            var dataentry = _repository.Get<DataEntry>().FirstOrDefault(e => e.Period == period);
            if (dataentry == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return dataentry;
        }

        public HttpResponseMessage Post(DataEntry dataentry)
        {
            if (ModelState.IsValid)
            {
                _repository.Create(dataentry);
              
                return Request.CreateResponse(HttpStatusCode.Created, dataentry);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
    }
}