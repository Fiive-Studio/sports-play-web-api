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

namespace FutbolPlay.Controllers
{
    [Authorize]
    [RoutePrefix("api/reservation_report")]
    public class reservation_reportController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        // GET: api/reservation_report/5
        [ResponseType(typeof(reservation_report))]
        public async Task<IHttpActionResult> Getreservation_report(int id)
        {
            var reservation = from a in db.reservation_report
                              join b in db.status_type on a.status equals b.id_status
                              where a.id_place == id
                              select new
                              {
                                  id_place = a.id_place,
                                  year = a.year,
                                  month = a.month,
                                  status = b.name.Trim(),
                                  cantidad = a.cantidad,
                                  ingresos = a.ingresos
                              };

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        // Reporte de Barras
        [Route("report_bar/{id}"), HttpGet]
        [ResponseType(typeof(reservation_report))]
        public async Task<IHttpActionResult> Getreport_bar(int id)
        {
            var reservation = from a in db.reservation_report
                              join b in db.status_type on a.status equals b.id_status
                              where a.id_place == id
                              group a by new
                              {
                                  a.year,                                  
                                  a.month,
                                  a.cantidad,
                                  a.ingresos,
                                  b.name
                              } into data
                              select new
                              {
                                  year = data.Key.year,
                                  month = data.Key.month,
                                  status = data.Key.name.Trim(),
                                  cantidad = data.Key.cantidad,
                                  ingresos = data.Key.ingresos
                              };

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }
    }
}