using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Silas.Domain;
using Silas.Server.DB;

namespace Silas.API.Controllers
{
    public class DataEntryController : ApiController
    {
        private DataEntryEmptyContext db = new DataEntryEmptyContext();

        // GET api/MutableDataEntry
        public IEnumerable<DataEntry> GetDataEntries()
        {
            return db.Entries.AsEnumerable();
        }

        // GET api/MutableDataEntry/5
        public DataEntry GetDataEntry(int id)
        {
            DataEntry dataentry = db.Entries.Find(id);
            if (dataentry == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return dataentry;
        }

        // PUT api/MutableDataEntry/5
        public HttpResponseMessage PutDataEntry(int id, DataEntry dataentry)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != dataentry.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(dataentry).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/MutableDataEntry
        public HttpResponseMessage PostDataEntry(DataEntry dataentry)
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

        // DELETE api/MutableDataEntry/5
        public HttpResponseMessage DeleteDataEntry(int id)
        {
            DataEntry dataentry = db.Entries.Find(id);
            if (dataentry == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Entries.Remove(dataentry);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dataentry);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}