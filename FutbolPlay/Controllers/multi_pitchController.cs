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
    public class multi_pitchController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        // GET: api/multi_pitch
        public IQueryable<multi_pitch> Getmulti_pitch()
        {
            return db.multi_pitch;
        }

        // GET: api/multi_pitch/5
        [ResponseType(typeof(multi_pitch))]
        public IHttpActionResult Getmulti_pitch(int id)
        {
            multi_pitch multi_pitch = db.multi_pitch.Find(id);
            if (multi_pitch == null)
            {
                return NotFound();
            }

            return Ok(multi_pitch);
        }

        // PUT: api/multi_pitch/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putmulti_pitch(int id, multi_pitch multi_pitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != multi_pitch.id_multi_pitch)
            {
                return BadRequest();
            }

            db.Entry(multi_pitch).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!multi_pitchExists(id))
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

        // POST: api/multi_pitch
        [ResponseType(typeof(multi_pitch))]
        public IHttpActionResult Postmulti_pitch(multi_pitch multi_pitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.multi_pitch.Add(multi_pitch);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = multi_pitch.id_multi_pitch }, multi_pitch);
        }

        // DELETE: api/multi_pitch/5
        [ResponseType(typeof(multi_pitch))]
        public IHttpActionResult Deletemulti_pitch(int id)
        {
            multi_pitch multi_pitch = db.multi_pitch.Find(id);
            if (multi_pitch == null)
            {
                return NotFound();
            }

            db.multi_pitch.Remove(multi_pitch);
            db.SaveChanges();

            return Ok(multi_pitch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool multi_pitchExists(int id)
        {
            return db.multi_pitch.Count(e => e.id_multi_pitch == id) > 0;
        }
    }
}