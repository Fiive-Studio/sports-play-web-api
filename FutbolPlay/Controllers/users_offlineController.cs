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
using FutbolPlay.Functions;

namespace FutbolPlay.Controllers
{
    [Authorize]
    public class users_offlineController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        // GET: api/users_offline
        public IQueryable<users_offline> Getusers_offline()
        {
            return db.users_offline;
        }

        // GET: api/users_offline/5
        [ResponseType(typeof(users_offline))]
        public IHttpActionResult Getusers_offline(int id)
        {
            users_offline users_offline = db.users_offline.Find(id);
            if (users_offline == null)
            {
                return NotFound();
            }

            return Ok(users_offline);
        }

        // GET: api/users_offline/5
        [ResponseType(typeof(users_offline))]
        [Route("api/users_offline/getbyplace/{id_place}"), HttpGet]
        public IHttpActionResult Getusers_offline_ByPlace(int id_place)
        {
            var listUserOffline = from a in db.users_offline
                                  where a.id_place.Equals(id_place)
                                  select a;

            if (listUserOffline == null)
            {
                return NotFound();
            }

            return Ok(listUserOffline);
        }

        // GET: api/users_offline/5
        [ResponseType(typeof(users_offline))]
        [Route("api/users_offline/getbyname/{name}"), HttpGet]
        public IHttpActionResult Getusers_offline_ByName(string name)
        {
            var listUserOffline = (from a in db.users_offline
                                   where a.name.ToLower().Contains(name.ToLower())
                                   select a).Take(20);

            if (listUserOffline == null)
            {
                return NotFound();
            }

            return Ok(listUserOffline);
        }

        // POST: api/users_offline/update/5
        [Route("api/users_offline/update/{id}"), HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putusers_offline(int id, users_offline users_offline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != users_offline.id_users_offline)
            {
                return BadRequest();
            }

            db.Entry(users_offline).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!users_offlineExists(id))
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

        // POST: api/users_offline
        [ResponseType(typeof(users_offline))]
        public IHttpActionResult Postusers_offline(users_offline users_offline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CommonFunctions.Log($"Va a guardar el usuario: {users_offline.name} - Telefono: {users_offline.phone}");

            // Validar si esta duplicado el numero de celular
            //var validateUsersOffline = (from a in db.users_offline
            //                            where a.phone.Trim().Equals(users_offline.phone) && a.id_place.Equals(users_offline.id_place)
            //                            select a).ToList();

            //if (validateUsersOffline != null && validateUsersOffline.Count > 0)
            //{
            //    CommonFunctions.Log($"Conflicto de numero de celular: {users_offline.phone}");
            //    return Conflict();
            //}

            db.users_offline.Add(users_offline);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = users_offline.id_users_offline }, users_offline);
        }

        // DELETE: api/users_offline/5
        [ResponseType(typeof(users_offline))]
        public IHttpActionResult Deleteusers_offline(int id)
        {
            users_offline users_offline = db.users_offline.Find(id);
            if (users_offline == null)
            {
                return NotFound();
            }

            db.users_offline.Remove(users_offline);
            db.SaveChanges();

            return Ok(users_offline);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool users_offlineExists(int id)
        {
            return db.users_offline.Count(e => e.id_users_offline == id) > 0;
        }
    }
}