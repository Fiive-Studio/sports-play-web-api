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
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using FutbolPlay.Data;
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using System.IO;
using FutbolPlay.Functions;
using System.Text.RegularExpressions;
using System.Configuration;

namespace FutbolPlay.Controllers
{
    public class usersController : ApiController
    {
        private FutPlayAppDB db = new FutPlayAppDB();
        CommonFunctions cf = new CommonFunctions();

        /// <summary>
        /// Obtener todos los usuarios
        /// </summary>
        /// <remarks>GET: api/users</remarks>
        /// <returns>Lista de usuarios creados en el sistema</returns>
        [Authorize]
        public IQueryable<users> Getusers()
        {
            return db.users;
        }

        // GET: api/users/5
        [Authorize]
        [Route("api/users/byname/{name}"), HttpGet]
        [ResponseType(typeof(users))]
        public IHttpActionResult Getusersbyname(string name)
        {
            var findUser = from a in db.users
                           where a.name.Contains(name) || a.email.Contains(name)
                           select a;

            if (findUser == null)
                return NotFound();

            return Ok(findUser);
        }

        [Authorize]
        [Route("api/users/bynameandidplace"), HttpPost]
        [ResponseType(typeof(user_result))]
        public IHttpActionResult GetUsersByNameAndIdPlace(users data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Se hace union de Usuarios registrados y Usuarios NO registrados, para los usuarios NO registrados
            // se hace filtro con el id de la cancha
            var findUser = (from a in db.users
                            where a.name.ToLower().Contains(data.name.ToLower())
                            select new user_result { id = a.id_users, name = a.name, email = a.email, phone = a.phone, id_user_type = 1 }).Concat
                            (from a in db.users_offline
                             where a.name.ToLower().Contains(data.name.ToLower()) && a.id_place == data.id_users_type || a.phone.Contains(data.name) && a.id_place == data.id_users_type
                             select new user_result { id = a.id_users_offline, name = a.name, email = a.email, phone = a.phone, id_user_type = 2 }).Take(10);

            if (findUser == null)
                return NotFound();

            return Ok(findUser);
        }

        // GET: api/users/5
        [Authorize]
        [ResponseType(typeof(users))]
        public IHttpActionResult Getusers(int id)
        {
            users users = db.users.Find(id);
            if (users == null)
                return NotFound();
            else
                users.password = null;

            return Ok(users);
        }

        /// <summary>
        /// Actualizar los datos de un usuario ya registrado (Correo y Telefono)
        /// </summary>
        /// <param name="id">Id del usuario</param>
        /// <param name="email">Correo electronico</param>
        /// <param name="phone">Telefono</param>
        /// <response code="006">Ya existe un usuario registrado con el correo y telefono iguales</response>
        /// <response code="004">Ya existe un usuario registrado con el correo igual</response>
        /// <response code="005">Ya existe un usuario registrado con el telefono igual</response>
        /// <response code="010">El id usuario enviado no existe en la base de datos</response>
        /// <response code="204">El proceso se ejecuto correctamente</response>
        /// <returns></returns>
        [Authorize]
        [Route("api/users/update/{id}"), HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putusers(int id, users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            #region Validation user db

            // Validamos que los datos a actualizar no existan para otro usuario como lo son el telefono y el correo
            var validateInfo = (from a in db.users
                                where (a.email.Equals(users.email.Trim()) || a.phone.Equals(users.phone.Trim())) && !a.id_users.Equals(id)
                                select a).FirstOrDefault();
            #endregion

            #region Validations Data

            if (validateInfo != null)
            {
                if (validateInfo.email == null)
                    validateInfo.email = string.Empty;

                if (validateInfo.phone == null)
                    validateInfo.phone = string.Empty;

                // Ya existe un usuario registrado con el correo y telefono iguales
                if (validateInfo.email.Equals(users.email.Trim()) && validateInfo.phone.Equals(users.phone.Trim()))
                    return BadRequest("006");

                // Ya existe un usuario registrado con el correo igual
                if (validateInfo.email.Equals(users.email.Trim()) && !validateInfo.phone.Equals(users.phone.Trim()))
                    return BadRequest("004");

                // Ya existe un usuario registrado con el telefono igual
                if (!validateInfo.email.Equals(users.email.Trim()) && validateInfo.phone.Equals(users.phone.Trim()))
                    return BadRequest("005");
            }

            #endregion

            #region Update user

            // Update data
            var updateUser = (from a in db.users
                              where a.id_users.Equals(id)
                              select a).FirstOrDefault();

            // El id usuario enviado no existe en la base de datos
            if (updateUser == null)
            { return BadRequest("010"); }

            try
            {
                updateUser.name = users.name;
                updateUser.email = users.email;
                updateUser.phone = users.phone;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!usersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            #endregion

            return StatusCode(HttpStatusCode.NoContent);
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
        [Route("api/users/access/{id}"), HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPassword(int id, changepassword change)
        {
            try
            {
                if (!usersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    string getSHA_OldPassword = cf.getSHA1(change.password_old);

                    var user = (from a in db.users
                                where a.id_users.Equals(id) && a.password.Equals(getSHA_OldPassword)
                                select a).FirstOrDefault();

                    if (user != null)
                    {
                        string getSHA_NewPassword = cf.getSHA1(change.password_new);
                        user.password = getSHA_NewPassword;
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
        /// Registrar un nuevo usuario en la aplicacion
        /// </summary>
        /// <remarks>POST: api/users</remarks>
        /// <param name="users">Datos del usuario</param>
        /// <response code="201">Created</response>
        /// <response code="400">BadRequest</response>
        /// <returns>Confirmacion de creacion del usuario</returns>
        [ResponseType(typeof(users))]
        public IHttpActionResult Postusers(users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(users.id_usersocialred))
            {
                #region Validation Field Empty

                if (string.IsNullOrEmpty(users.name))
                    return BadRequest("011");

                if (string.IsNullOrEmpty(users.password))
                    return BadRequest("011");

                if (string.IsNullOrEmpty(users.email))
                    return BadRequest("011");

                string numberPhone = Regex.Match(users.phone, @"\d+").Value;
                users.phone = numberPhone;
                if (string.IsNullOrEmpty(users.phone))
                    return BadRequest("011");

                #endregion

                #region Validation Field Type

                if (!cf.ValidateEmail(users.email))
                { return BadRequest("018"); }

                #endregion

                #region Find users db

                // Validamos si ya existe el usuario por correo o por mail
                users validateInfo = (from a in db.users
                                      where (a.email.Equals(users.email.Trim()) || a.phone.Equals(users.phone.Trim()))
                                      select a).FirstOrDefault();
                #endregion

                #region Validations Data

                if (validateInfo != null)
                {
                    if (users.email == null)
                        users.email = string.Empty;

                    if (users.phone == null)
                        users.phone = string.Empty;

                    if (validateInfo.email == null)
                        validateInfo.email = string.Empty;

                    if (validateInfo.phone == null)
                        validateInfo.phone = string.Empty;

                    // Ya existe un usuario registrado con el correo y telefono iguales
                    if (validateInfo.email.Equals(users.email.Trim()) && validateInfo.phone.Equals(users.phone.Trim()))
                        return BadRequest("006");

                    // Ya existe un usuario registrado con el correo igual
                    if (validateInfo.email.Equals(users.email.Trim()) && !validateInfo.phone.Equals(users.phone.Trim()))
                        return BadRequest("004");

                    // Ya existe un usuario registrado con el telefono igual
                    if (!validateInfo.email.Equals(users.email.Trim()) && validateInfo.phone.Equals(users.phone.Trim()))
                        return BadRequest("005");
                }

                #endregion

                string getPassword = cf.getSHA1(users.password);
                users.password = getPassword;
            }
            else
            {
                #region Validation Field Empty

                if (string.IsNullOrEmpty(users.name))
                    return BadRequest("011");
                #endregion

                // En caso que se envie el Id_usuerFacebook se valida que ya no existe en el sistema
                var validateIdFacebook = (from a in db.users
                                          where users.id_usersocialred != null && a.id_usersocialred.Equals(users.id_usersocialred)
                                          select a.name).FirstOrDefault();

                if (validateIdFacebook != null)
                {
                    return StatusCode(HttpStatusCode.Created);
                }

                var validateMail = (from a in db.users
                                    where a.email.Equals(users.email)
                                    select a.name).FirstOrDefault();

                if (validateMail != null)
                { users.email = string.Empty; }
            }

            db.users.Add(users);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = users.id_users }, users);
        }

        // DELETE: api/users/5
        [Authorize]
        [ResponseType(typeof(users))]
        public IHttpActionResult Deleteusers(int id)
        {
            users users = db.users.Find(id);
            if (users == null)
            {
                return NotFound();
            }

            db.users.Remove(users);
            db.SaveChanges();

            return Ok(users);
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

            var audience = "099153c2625149bc8ecb3e85e03f0022";
            var secret = TextEncodings.Base64Url.Decode("4e686af7bdcc5ae005a247624fd8c7283257c2514f6b3ad2ff5d4cb6d95196e6");
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

        [ResponseType(typeof(users))]
        [Route("api/users/forgotpassword"), HttpPost]
        public IHttpActionResult ForgotPassword(users user)
        {
            // Validamos si el user existe.
            var existUsers = (from a in db.users
                              where a.email.Equals(user.email)
                              select a).FirstOrDefault();

            if (existUsers == null)
            { return BadRequest("008"); }

            try
            {
                if (!string.IsNullOrEmpty(existUsers.id_usersocialred))
                { return BadRequest("014"); }

                string numberKeyGenerated;
                existUsers.password = cf.generateNewPassword(out numberKeyGenerated);
                if (cf.sendMail(existUsers, numberKeyGenerated))
                    db.SaveChanges();
                else
                    return BadRequest("009");

            }
            catch (Exception)
            {
                return BadRequest();
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

        private bool usersExists(int id)
        {
            return db.users.Count(e => e.id_users == id) > 0;
        }
    }
}