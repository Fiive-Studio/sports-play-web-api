using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using FutbolPlay;
using FutbolPlay.Functions;
using System.Globalization;

namespace FutbolPlay.Controllers
{
    //    [Authorize]
    public class reportsController : ApiController
    {
        FutPlayAppDB db = new FutPlayAppDB();
        CommonFunctions cf = new CommonFunctions();

        // GET: api/reports
        [Route("api/reports/resumenreservation/{id}")]
        [ResponseType(typeof(reservation))]
        public async Task<IHttpActionResult> Getresumenreservation(int id)
        {
            DateTime dateNow = DateTime.Now;
            int diff = dateNow.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
            { diff += 7; }

            DateTime startDateWeek = dateNow.AddDays(-1 * diff).Date;
            DateTime endDateWeek = startDateWeek.AddDays(6).Date;

            //var resumenReservation = from a in db.reservation
            //                         where a.place = id

            return Ok(ddd);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}