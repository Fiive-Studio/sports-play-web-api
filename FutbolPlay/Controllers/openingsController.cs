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
    public class openingsController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        // GET: api/openings
        public IQueryable<opening> Getopening()
        {
            return db.opening;
        }

        // GET: api/openings/5
        [ResponseType(typeof(opening))]
        public IHttpActionResult Getopening(int id)
        {
            opening opening = db.opening.Find(id);
            if (opening == null)
            {
                return NotFound();
            }

            return Ok(opening);
        }

        // GET: api/openings/5
        [Route("api/openings/openingbyplace/{id}")]
        [ResponseType(typeof(opening))]
        public IHttpActionResult GetopeningByPlace(int id)
        {
            var opening = from a in db.opening
                          where a.id_place == id
                          select a;

            if (opening == null)
            {
                return NotFound();
            }

            return Ok(opening);
        }

        // PUT: api/openings/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putopening(int id, opening opening)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != opening.id_opening)
            {
                return BadRequest();
            }

            db.Entry(opening).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!openingExists(id))
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

        // POST: api/openings
        [ResponseType(typeof(opening))]
        public IHttpActionResult Postopening(opening opening)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.opening.Add(opening);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = opening.id_opening }, opening);
        }

        // DELETE: api/openings/5
        [ResponseType(typeof(opening))]
        public IHttpActionResult Deleteopening(int id)
        {
            opening opening = db.opening.Find(id);
            if (opening == null)
            {
                return NotFound();
            }

            db.opening.Remove(opening);
            db.SaveChanges();

            return Ok(opening);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool openingExists(int id)
        {
            return db.opening.Count(e => e.id_opening == id) > 0;
        }
    }
}