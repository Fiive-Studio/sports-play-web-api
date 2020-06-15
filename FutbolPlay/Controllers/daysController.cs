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
    [Authorize]
    [RoutePrefix("api/days")]
    public class daysController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        // GET: api/days
        public IQueryable<days> Getdays()
        {
            return db.days;
        }

        // GET: api/days/5
        [ResponseType(typeof(days))]
        public IHttpActionResult Getdays(int id)
        {
            days days = db.days.Find(id);
            if (days == null)
            {
                return NotFound();
            }

            return Ok(days);
        }

        // PUT: api/days/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putdays(int id, days days)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != days.id_day)
            {
                return BadRequest();
            }

            db.Entry(days).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!daysExists(id))
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

        // POST: api/days
        [ResponseType(typeof(days))]
        public IHttpActionResult Postdays(days days)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.days.Add(days);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = days.id_day }, days);
        }

        // DELETE: api/days/5
        [ResponseType(typeof(days))]
        public IHttpActionResult Deletedays(int id)
        {
            days days = db.days.Find(id);
            if (days == null)
            {
                return NotFound();
            }

            db.days.Remove(days);
            db.SaveChanges();

            return Ok(days);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool daysExists(int id)
        {
            return db.days.Count(e => e.id_day == id) > 0;
        }
    }
}