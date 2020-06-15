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
    public class holidaysController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        // GET: api/holidays
        public IQueryable<holidays> Getholidays()
        {
            return db.holidays;
        }

        // GET: api/holidays/5
        [ResponseType(typeof(holidays))]
        public IHttpActionResult Getholidays(int id)
        {
            holidays holidays = db.holidays.Find(id);
            if (holidays == null)
            {
                return NotFound();
            }

            return Ok(holidays);
        }

        // GET: api/holidays/5
        [Authorize]
        [Route("api/holidays/isholiday/{date}"), HttpGet]
        [ResponseType(typeof(holidays))]
        public IHttpActionResult GetIsholidays(DateTime date)
        {
            DateTime dt;
            DateTime.TryParse(date.ToString("yyyy-MM-dd"), out dt);

            int? findHoliday = (from a in db.holidays
                                where a.date.Equals(dt.Date)
                                select a.id_holiday).FirstOrDefault();

            if (findHoliday == null || findHoliday == 0)
            { return BadRequest("false"); }
            else
            { return Ok("True"); }
        }

        // PUT: api/holidays/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putholidays(int id, holidays holidays)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != holidays.id_holiday)
            {
                return BadRequest();
            }

            db.Entry(holidays).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!holidaysExists(id))
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

        // POST: api/holidays
        [ResponseType(typeof(holidays))]
        public IHttpActionResult Postholidays(holidays holidays)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.holidays.Add(holidays);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (holidaysExists(holidays.id_holiday))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = holidays.id_holiday }, holidays);
        }

        // DELETE: api/holidays/5
        [ResponseType(typeof(holidays))]
        public IHttpActionResult Deleteholidays(int id)
        {
            holidays holidays = db.holidays.Find(id);
            if (holidays == null)
            {
                return NotFound();
            }

            db.holidays.Remove(holidays);
            db.SaveChanges();

            return Ok(holidays);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool holidaysExists(int id)
        {
            return db.holidays.Count(e => e.id_holiday == id) > 0;
        }
    }
}