using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
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
            return db.Entries.AsEnumerable();
        }

        public DataEntry Get(int period)
        {
            DataEntry dataentry = db.Entries.FirstOrDefault(e => e.Period == period);
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
                db.Entries.Add(dataentry);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, dataentry);
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