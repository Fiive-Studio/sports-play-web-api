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

namespace FutbolPlay.Controllers
{
    public class placesController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        /// <summary>
        /// Obtener lista de establecimientos de canchas sinteticas
        /// </summary>
        /// <remarks>Metodo que obtiene el listado de establecimientos de canchas sinteticas con nombre y direccion</remarks>
        /// <response code="200">Ok</response>
        /// <returns>Lista de establecimientos de canchas sinteticas</returns>
        public IHttpActionResult Getplaces()
        {
            var place = (from a in db.place
                         where a.status == true
                         select new
                         {
                             a.id_place,
                             a.name,
                             a.phone,
                             a.description,
                             a.address,
                             a.status,
                             a.latitude,
                             a.longitude,
                             a.max_days_reservation,
                             a.autoconfirm,
                             a.format_hour,
                             a.profile_img,
                             a.max_time_cancelation,
                             hours = (from d in db.opening
                                      where d.id_place.Equals(a.id_place)
                                      select d).ToList()
                         }).OrderBy(x => Guid.NewGuid()).Take(50);

            return Ok(place);
        }

        /// <summary>
        /// Valida si un place maneja canchas multuples
        /// </summary>
        /// <remarks>Toma el id place y valida si maneja canchas multiples</remarks>
        /// <response code="200">Ok</response>
        /// <returns>True si maneja canchas muntilples de lo contrario false</returns>
        [Authorize]
        [Route("api/places/getplacesismultiple/{id}")]
        public IHttpActionResult getplacesismultiple(int id)
        {
            int? pitch = (from a in db.pitch
                          where a.id_place == id && a.id_pitch_type == 2 && a.status == true
                          select a.id_pitch).FirstOrDefault();

            if (pitch > 0)
            { return Ok("True"); }
            else
            { return Ok("False"); }
        }

        [Authorize]
        [Route("api/places/getplacesistest/{id}")]
        public IHttpActionResult GetPlaceIsTest(int id)
        {
            var place = (from p in db.place where p.id_place.Equals(id) && p.end_date_test > DateTime.Now select p.end_date_test);

            if (place != null && place.Count()>0)
            { return Ok("True"); }
            else
            { return Ok("False"); }
        }

        /// <summary>
        /// Obtiene los datos completos de un establecimiento de canchas sinteticas
        /// </summary>
        /// <param name="id">Id del establecimiento</param>
        /// <remarks>Metodo que obtiene los datos completos de un establecimiento de canchas sinteticas</remarks>
        /// <response code="200">Ok</response>
        /// <response code="404">NotFound</response>
        /// <returns>Datos del establicimiento de canchas sinteticas</returns>
        [Authorize]
        [ResponseType(typeof(place))]
        public IHttpActionResult GetPlace(int id)
        {
            var place = (from a in db.place
                         where a.status == true && a.id_place == id
                         select new
                         {
                             a.id_place,
                             a.name,
                             a.phone,
                             a.description,
                             a.address,
                             a.status,
                             a.latitude,
                             a.longitude,
                             a.max_days_reservation,
                             a.autoconfirm,
                             a.format_hour,
                             a.max_time_cancelation,
                             hours = (from d in db.opening
                                      where d.id_place.Equals(a.id_place)
                                      select d).ToList()
                         }).OrderBy(x => Guid.NewGuid()).Take(50);

            if (place == null)
            {
                return NotFound();
            }

            return Ok(place);
        }

        /// <summary>
        /// Obtiene los datos de ubicacion de un establecimiento de canchas sinteticas
        /// </summary>
        /// <param name="id">Id del establecimiento</param>
        /// <remarks>Metodo que obtiene los datos de Latitud y Longitud para la ubicacion exacta del sitio en Maps</remarks>
        /// <response code="200">Ok</response>
        /// <response code="404">NotFound</response>
        /// <returns>Coordenadas para la ubicacion del establecimiento en Maps</returns>
        [Authorize]
        [Route("api/places/location/{id}")]
        [ResponseType(typeof(place))]
        public IHttpActionResult GetLocation(int id)
        {
            IQueryable<object> place = from a in db.place
                                       where a.id_place.Equals(id)
                                       select new { latitud = a.latitude, longitud = a.longitude, nombre = a.name };
            if (place == null)
            {
                return NotFound();
            }

            return Ok(place);
        }

        /// <summary>
        /// Modifica los datos del establecimiento de canchas sinteticas
        /// </summary>
        /// <param name="id">Id del establecimiento</param>
        /// <param name="place">Datos a Modificar del Establecimiento</param>
        /// <remarks>Metodo que modifica los datos de un establecimiento de canchas sinteticas. PUT: api/places/5</remarks>
        /// <response code="200">Ok</response>
        /// <response code="400">BadRequest</response>
        /// <response code="404">NotFound</response>
        /// <returns></returns>
        [Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putplace(int id, place place)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != place.id_place)
            {
                return BadRequest();
            }

            db.Entry(place).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!placeExists(id))
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

        /// <summary>
        /// Crea un nuevo establecimiento de canchas sinteticas
        /// </summary>
        /// <remarks>POST: api/places</remarks>
        /// <param name="place">Datos del establecimiento de canchas sinteticas</param>
        /// <response code="200">Ok</response>
        /// <response code="400">BadRequest</response>
        /// <returns>Id del Establecimiento creado y los datos enviados</returns>
        [Authorize]
        [ResponseType(typeof(place))]
        [HttpPost]
        public IHttpActionResult Postplace(place place)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.place.Add(place);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = place.id_place }, place);
        }

        /// <summary>
        /// Eliminar un establecimiento de canchas sinteticas
        /// </summary>
        /// <remarks>DELETE: api/places/5</remarks>
        /// <param name="id">Id del establecimiento</param>
        /// <response code="200">Ok</response>
        /// <response code="404">NotFound</response>
        /// <returns>Datos del establecimiento eliminado</returns>
        [Authorize]
        [ResponseType(typeof(place))]
        public IHttpActionResult Deleteplace(int id)
        {
            place place = db.place.Find(id);
            if (place == null)
            {
                return NotFound();
            }

            db.place.Remove(place);
            db.SaveChanges();

            return Ok(place);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool placeExists(int id)
        {
            return db.place.Count(e => e.id_place == id) > 0;
        }
    }
}