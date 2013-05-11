using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Silas.Domain;
using Silas.Server.DB;

namespace Silas.API.Controllers
{
    public class LiveDataController : ApiController
    {
        private readonly LiveDataContext db = new LiveDataContext();

        // GET api/MutableDataEntry
        public IEnumerable<DataEntry> GetAll()
        {
            return db.Entries.AsEnumerable();
        }

        // GET api/MutableDataEntry/5
        public DataEntry Get(int id)
        {
            DataEntry dataentry = db.Entries.Find(id);
            if (dataentry == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return dataentry;
        }

        public DataEntry GetLatestEntry()
        {
            return db.Entries.OrderByDescending(e => e.Id).FirstOrDefault();
        }

        // POST api/MutableDataEntry
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