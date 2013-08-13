using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Silas.Forecast.Models;
using Silas.Server.DB;

namespace Silas.API.Controllers
{
    public class ForecastController : ApiController
    {
        private readonly ForecastContext db = new ForecastContext();

        public IEnumerable<DataEntry> Get()
        {
            return db.DataEntries.AsEnumerable();
        }

        public DataEntry Get(int period)
        {
            var dataentry = db.DataEntries.FirstOrDefault(e => e.Period == period);
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
                db.DataEntries.Add(dataentry);
                db.SaveChanges();

                var response = Request.CreateResponse(HttpStatusCode.Created, dataentry);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}