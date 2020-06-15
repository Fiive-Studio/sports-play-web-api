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
    [RoutePrefix("api/pitches")]
    public class pitchesController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        // GET: api/pitches
        [Route("single/{id}"), HttpGet]
        [ResponseType(typeof(pitch))]
        public IQueryable<pitch> GetpitchSingle(int id)
        {
            var pitch = from a in db.pitch
                        where a.id_pitch_type == 1 && a.id_place == id
                        select a;

            return pitch;
        }

        [Route("multiple/{id}"), HttpGet]
        [ResponseType(typeof(pitch))]
        public IQueryable<pitch> GetpitchMuliple(int id)
        {
            var pitch = from a in db.pitch
                        where a.id_pitch_type == 2 && a.id_place == id
                        select a;

            return pitch;
        }

        // GET: api/pitches/5
        [ResponseType(typeof(pitch))]
        public IHttpActionResult Getpitch(int id)
        {
            var pitch = from a in db.pitch
                        where a.id_place == id
                        select new { id_pitch = a.id_pitch, name = a.description };

            if (pitch == null)
            {
                return NotFound();
            }

            return Ok(pitch);
        }

        // PUT: api/pitches/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putpitch(int id, pitch pitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pitch.id_pitch)
            {
                return BadRequest();
            }

            db.Entry(pitch).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!pitchExists(id))
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

        // POST: api/pitches
        [ResponseType(typeof(pitch))]
        public IHttpActionResult Postpitch(pitch pitch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.pitch.Add(pitch);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = pitch.id_pitch }, pitch);
        }

        // DELETE: api/pitches/5
        [ResponseType(typeof(pitch))]
        public IHttpActionResult Deletepitch(int id)
        {
            pitch pitch = db.pitch.Find(id);
            if (pitch == null)
            {
                return NotFound();
            }

            db.pitch.Remove(pitch);
            db.SaveChanges();

            return Ok(pitch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool pitchExists(int id)
        {
            return db.pitch.Count(e => e.id_pitch == id) > 0;
        }
    }
}