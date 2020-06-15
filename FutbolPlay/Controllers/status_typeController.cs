using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using FutbolPlay;

namespace FutbolPlay.Controllers
{
    public class status_typeController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        // GET: api/status_type
        public IQueryable<status_type> Getstatus_type()
        {
            return db.status_type;
        }

        // GET: api/status_type
        [Route("api/status_type/customers")]
        public IQueryable<status_type> Getstatus_typeCustomers()
        {
            var status = from a in db.status_type
                         where a.id_status != 3
                         select a;

            return status;
        }

        // GET: api/status_type/5
        [ResponseType(typeof(status_type))]
        public IHttpActionResult Getstatus_type(int id)
        {
            status_type status_type = db.status_type.Find(id);
            if (status_type == null)
            {
                return NotFound();
            }

            return Ok(status_type);
        }

        // PUT: api/status_type/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putstatus_type(int id, status_type status_type)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != status_type.id_status)
            {
                return BadRequest();
            }

            db.Entry(status_type).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!status_typeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/status_type
        [ResponseType(typeof(status_type))]
        public IHttpActionResult Poststatus_type(status_type status_type)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.status_type.Add(status_type);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = status_type.id_status }, status_type);
        }

        // DELETE: api/status_type/5
        [ResponseType(typeof(status_type))]
        public IHttpActionResult Deletestatus_type(int id)
        {
            status_type status_type = db.status_type.Find(id);
            if (status_type == null)
            {
                return NotFound();
            }

            db.status_type.Remove(status_type);
            db.SaveChanges();

            return Ok(status_type);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool status_typeExists(int id)
        {
            return db.status_type.Count(e => e.id_status == id) > 0;
        }
    }
}