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
using FutbolPlay.Data;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;
using System.Configuration;

namespace FutbolPlay.Controllers
{

    [RoutePrefix("api/customers")]
    public class customersController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();
        CommonFunctions cf = new CommonFunctions();

        // GET: api/customers
        [Authorize]
        public IQueryable<customer> Getcustomer()
        {
            return db.customer;
        }

        // GET: api/customers/5
        [Authorize]
        [ResponseType(typeof(customer))]
        public IHttpActionResult Getcustomer(int id)
        {
            customer customer = db.customer.Find(id);
            if (customer == null)
                return NotFound();
            else
                customer.password = null;

            return Ok(customer);
        }

        // PUT: api/customers/5
        [Authorize]
        [Route("update/{id}"), HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult PostCustomerUpdate(int id, customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            #region Validation customer db

            // Validamos que los datos a actualizar no existan para otro usuario como lo son el telefono y el correo
            var validateInfo = (from a in db.customer
                                      where a.email.Equals(customer.email.Trim()) && !a.id_customer.Equals(id)
                                      select a).FirstOrDefault();
            #endregion

            if (validateInfo != null)
            {
                if (validateInfo.email == null)
                    validateInfo.email = string.Empty;

                // Ya existe un usuario registrado con el correo igual
                if (validateInfo.email.Equals(customer.email.Trim()))
                    return BadRequest("004");
            }

            // Update data
            var updateUser = (from a in db.customer
                                    where a.id_customer.Equals(id)
                                    select a).FirstOrDefault();

            // El id usuario enviado no existe en la base de datos
            if (updateUser == null)
            { return BadRequest("010"); }

            try
            {
                updateUser.full_name = customer.full_name;
                updateUser.email = customer.email;
                updateUser.phone = customer.phone;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!customerExists(id))
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

        // POST: api/customers
        [Authorize]
        [ResponseType(typeof(customer))]
        public IHttpActionResult Postcustomer(customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.customer.Add(customer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = customer.id_customer }, customer);
        }

        // DELETE: api/customers/5
        [Authorize]
        [ResponseType(typeof(customer))]
        public IHttpActionResult Deletecustomer(int id)
        {
            customer customer = db.customer.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.customer.Remove(customer);
            db.SaveChanges();

            return Ok(customer);
        }

        [ResponseType(typeof(users))]
        [Route("forgotpassword"), HttpPost]
        public IHttpActionResult ForgotPassword(customer customers)
        {
            // Validamos si el user existe.
            var existCustomer = (from a in db.customer
                                 where a.email.Equals(customers.email)
                                 select a).FirstOrDefault();

            if (existCustomer == null)
            { return BadRequest("008"); }

            try
            {
                string numberKeyGenerated;
                existCustomer.password = cf.generateNewPassword(out numberKeyGenerated);
                if (cf.sendMail(existCustomer, numberKeyGenerated))
                { db.SaveChanges(); }
                else
                { return BadRequest("009"); }
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// Actualizar el password de un usuario
        /// </summary>
        /// <remarks>Metodo que actualiza el password de un cliente</remarks>
        /// <param name="id">Id del usuario</param>
        /// <param name="users">Nuevo password ingresado</param>
        /// <response code="204">No Content (Ok)</response>
        /// <response code="404">NotFound</response>
        /// <returns>Confirmacion de la creacion</returns>
        [Authorize]
        [Route("access/{id}"), HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPassword(int id, changepassword change)
        {
            try
            {
                if (!customerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    string getSHA_OldPassword = cf.getSHA1(change.password_old);

                    var customers = (from a in db.customer
                                     where a.id_customer.Equals(id) && a.password.Equals(getSHA_OldPassword)
                                     select a).FirstOrDefault();

                    if (customers != null)
                    {
                        string getSHA_NewPassword = cf.getSHA1(change.password_new);
                        customers.password = getSHA_NewPassword;
                        db.SaveChanges();
                    }
                    else
                    {
                        return BadRequest("012");
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Validar un token enviado por el usuario
        /// </summary>
        /// <remarks>Validar Token</remarks>
        /// <param name="name">Valor del Token a Validar</param>
        /// <response code="200">Ok</response>
        /// <response code="400">BadRequest</response>
        /// <returns>200 si es correcto, de lo contrario 400</returns>
        [Route("validatetoken"), HttpPost]
        public IHttpActionResult ValidateToken(users token)
        {
            if (string.IsNullOrEmpty(token.name))
            { return BadRequest("El token a validar se encuentra vacio."); }

            var issuer = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["currentApiSecurity"]];

            var audience = "cdb59355f3ba293977fc0945fb85f118";
            var secret = TextEncodings.Base64Url.Decode("d4f0bc5a29de06b510f9aa428f1eedba926012b591fef7a518e776a7c9bd1824");
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningToken = new BinarySecretSecurityToken(secret),
                ValidAudience = audience,
                ValidIssuer = issuer,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;
            try
            {
                tokenHandler.ValidateToken(token.name, validationParameters, out validatedToken);
            }
            catch (Exception)
            {
                return BadRequest("013");
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool customerExists(int id)
        {
            return db.customer.Count(e => e.id_customer == id) > 0;
        }

    }

}