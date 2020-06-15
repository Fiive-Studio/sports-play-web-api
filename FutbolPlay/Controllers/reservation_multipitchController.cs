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
    public class reservation_multipitchController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        // GET: api/reservation_multipitch
        public IQueryable<reservation_multipitch> Getreservation_multipitch()
        {
            return db.reservation_multipitch;
        }

        // GET: api/reservation_multipitch/5
        [ResponseType(typeof(reservation_multipitch))]
        public IHttpActionResult Getreservation_multipitch(int id)
        {
            reservation_multipitch reservation_multipitch = db.reservation_multipitch.Find(id);
            if (reservation_multipitch == null)
            {
                return NotFound();
            }

            return Ok(reservation_multipitch);
        }

        // PUT: api/reservation_multipitch/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putreservation_multipitch(int id, reservation_multipitch reservation_multipitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reservation_multipitch.id_reservation_multipitch)
            {
                return BadRequest();
            }

            db.Entry(reservation_multipitch).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!reservation_multipitchExists(id))
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

        // POST: api/reservation_multipitch
        [ResponseType(typeof(reservation_multipitch))]
        public IHttpActionResult Postreservation_multipitch(reservation_multipitch reservation_multipitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.reservation_multipitch.Add(reservation_multipitch);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = reservation_multipitch.id_reservation_multipitch }, reservation_multipitch);
        }

        // DELETE: api/reservation_multipitch/5
        [ResponseType(typeof(reservation_multipitch))]
        public IHttpActionResult Deletereservation_multipitch(int id)
        {
            reservation_multipitch reservation_multipitch = db.reservation_multipitch.Find(id);
            if (reservation_multipitch == null)
            {
                return NotFound();
            }

            db.reservation_multipitch.Remove(reservation_multipitch);
            db.SaveChanges();

            return Ok(reservation_multipitch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool reservation_multipitchExists(int id)
        {
            return db.reservation_multipitch.Count(e => e.id_reservation_multipitch == id) > 0;
        }
    }
}