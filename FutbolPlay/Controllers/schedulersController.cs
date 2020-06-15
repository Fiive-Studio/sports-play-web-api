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
using System.Data.Entity.Core.Objects;
using FutbolPlay.Functions;

namespace FutbolPlay.Controllers
{
    [Authorize]
    [RoutePrefix("api/schedulers")]
    public class schedulersController : ApiController
    {
        FutPlayAppDB db = new FutPlayAppDB();
        CommonFunctions cf = new CommonFunctions();

        // GET: api/schedulers
        public IQueryable<scheduler> Getscheduler()
        {
            return db.scheduler;
        }

        // GET: api/schedulers/5
        [ResponseType(typeof(scheduler))]
        public IHttpActionResult Getscheduler(int id)
        {
            scheduler scheduler = db.scheduler.Find(id);
            if (scheduler == null)
            {
                return NotFound();
            }

            return Ok(scheduler);
        }

        [Route("datenow"), HttpGet]
        public IHttpActionResult PostPricePitch()
        {
            DateTime dt = cf.GetDate();
            return Ok(dt.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        // POST: api/schedulers
        [ResponseType(typeof(scheduler))]
        [Route("price"), HttpPost]
        public IHttpActionResult PostPricePitch(scheduler scheduler)
        {
            if (scheduler == null)
            { return BadRequest(); }

            if (scheduler.hour.Hours == 0 || scheduler.id_pitch == 0 || scheduler.date_insert.Year == 1)
            { return BadRequest(); }

            IQueryable<object> getPrice = null;
            DateTime dateEntry = scheduler.date_insert.Date;

            holidays holiday = (from a in db.holidays
                                where a.date == dateEntry
                                select a).FirstOrDefault();

            if (holiday != null)
            {
                getPrice = (from a in db.scheduler
                            where a.id_day == 8 &&
                                  a.id_pitch == scheduler.id_pitch &&
                                  a.hour.Hours == scheduler.hour.Hours
                            select new { id_pitch = a.id_pitch, value = a.value });
            }
            else
            {
                int intDayOfWeek = scheduler.date_insert.DayOfWeek.GetHashCode();
                if (intDayOfWeek == 0) { intDayOfWeek = 7; }

                getPrice = (from a in db.scheduler
                            where a.id_day == intDayOfWeek &&
                                  a.id_pitch == scheduler.id_pitch &&
                                  a.hour.Hours == scheduler.hour.Hours
                            select new { id_pitch = a.id_pitch, value = a.value });
            }

            return Ok(getPrice);
        }

        // PUT: api/schedulers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putscheduler(int id, scheduler scheduler)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != scheduler.id_schedule)
            {
                return BadRequest();
            }

            db.Entry(scheduler).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!schedulerExists(id))
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

        // POST: api/schedulers
        [ResponseType(typeof(scheduler))]
        public IHttpActionResult Postscheduler(scheduler scheduler)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.scheduler.Add(scheduler);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (schedulerExists(scheduler.id_schedule))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = scheduler.id_schedule }, scheduler);
        }

        // DELETE: api/schedulers/5
        [ResponseType(typeof(scheduler))]
        public IHttpActionResult Deletescheduler(int id)
        {
            scheduler scheduler = db.scheduler.Find(id);
            if (scheduler == null)
            {
                return NotFound();
            }

            db.scheduler.Remove(scheduler);
            db.SaveChanges();

            return Ok(scheduler);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool schedulerExists(int id)
        {
            return db.scheduler.Count(e => e.id_schedule == id) > 0;
        }
    }
}