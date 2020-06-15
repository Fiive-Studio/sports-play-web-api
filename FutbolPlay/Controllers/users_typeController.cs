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
    public class users_typeController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();

        // GET: api/users_type
        public IQueryable<users_type> Getusers_type()
        {
            return db.users_type;
        }

        // GET: api/users_type/5
        [ResponseType(typeof(users_type))]
        public IHttpActionResult Getusers_type(int id)
        {
            users_type users_type = db.users_type.Find(id);
            if (users_type == null)
            {
                return NotFound();
            }

            return Ok(users_type);
        }

        // PUT: api/users_type/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putusers_type(int id, users_type users_type)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != users_type.id_users_type)
            {
                return BadRequest();
            }

            db.Entry(users_type).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!users_typeExists(id))
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

        // POST: api/users_type
        [ResponseType(typeof(users_type))]
        public IHttpActionResult Postusers_type(users_type users_type)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.users_type.Add(users_type);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = users_type.id_users_type }, users_type);
        }

        // DELETE: api/users_type/5
        [ResponseType(typeof(users_type))]
        public IHttpActionResult Deleteusers_type(int id)
        {
            users_type users_type = db.users_type.Find(id);
            if (users_type == null)
            {
                return NotFound();
            }

            db.users_type.Remove(users_type);
            db.SaveChanges();

            return Ok(users_type);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool users_typeExists(int id)
        {
            return db.users_type.Count(e => e.id_users_type == id) > 0;
        }
    }
}