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
using System.Threading.Tasks;

namespace FutbolPlay.Controllers
{
    [Authorize]
    [RoutePrefix("api/reservations")]
    public class reservationsController : ApiController
    {
        FutPlayAppDB db = new FutPlayAppDB();
        CommonFunctions cf = new CommonFunctions();

        // GET: api/reservations
        public IQueryable<reservation> Getreservation()
        {
            return db.reservation;
        }

        // GET: api/reservations/5
        [ResponseType(typeof(reservation))]
        public IHttpActionResult Getreservation(int id)
        {
            reservation reservation = db.reservation.Find(id);
            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        // GET: api/reservations/5
        [Route("detail/{id}")]
        [ResponseType(typeof(reservation))]
        public IHttpActionResult Getreservationdetail(int id)
        {
            List<object> lstUserData = new List<object>();
            reservation detailReservation = (from a in db.reservation
                                             where a.id_reservation == id
                                             select a).FirstOrDefault();

            if (detailReservation.id_users_offline != null)
            {
                var infoUser = (from a in db.reservation
                                join b in db.pitch on a.id_pitch equals b.id_pitch
                                join c in db.scheduler on a.id_scheduler equals c.id_schedule
                                join d in db.users_offline on a.id_users_offline equals d.id_users_offline
                                where a.id_reservation == id
                                select new
                                {
                                    hour = a.hour,
                                    description = b.description,
                                    value = (a.value.Equals(null) ? c.value : a.value),
                                    name = d.name,
                                    phone = d.phone,
                                    a.id_reservation,
                                    date = a.date,
                                    a.status,
                                    reservation_description = a.description
                                }).ToList();

                return Ok(infoUser);
            }


            if (detailReservation.id_users != null)
            {
                var infoUser = (from a in db.reservation
                                join b in db.pitch on a.id_pitch equals b.id_pitch
                                join c in db.scheduler on a.id_scheduler equals c.id_schedule
                                join d in db.users on a.id_users equals d.id_users
                                where a.id_reservation == id
                                select new
                                {
                                    hour = a.hour,
                                    description = b.description,
                                    value = (a.value.Equals(null) ? c.value : a.value),
                                    name = d.name,
                                    phone = d.phone,
                                    a.id_reservation,
                                    date = a.date,
                                    a.status,
                                    reservation_description = a.description
                                }).ToList();

                return Ok(infoUser);
            }

            return BadRequest();
        }

        // GET: api/reservations/5
        [Route("myreservations/{id_user}")]
        [ResponseType(typeof(reservation))]
        public IHttpActionResult Getmyreservation(int id_user)
        {
            var myReservation = (from a in db.reservation
                                 join b in db.place on a.id_place equals b.id_place
                                 join c in db.scheduler on a.id_scheduler equals c.id_schedule
                                 join p in db.pitch on a.id_pitch equals p.id_pitch
                                 where a.id_users == id_user
                                 orderby a.date descending
                                 select new
                                 {
                                     name_place = b.name,
                                     id_place = a.id_place,
                                     hour = a.hour,
                                     date = a.date,
                                     value = c.value,
                                     status = a.status,
                                     id_reservation = a.id_reservation,
                                     b.address,
                                     b.profile_img,
                                     pitch_description = p.description
                                 }).Take(10);

            return Ok(myReservation);
        }

        // GET: My reserversations pending 
        [Route("customer/myreservations/{id_place}")]
        [ResponseType(typeof(reservation))]
        public IHttpActionResult Getreservationpending(int id_place)
        {
            // Se coloca asi para poder deja la hora, minuto y segundo en 0, ya que si se usa directamente el DateTime tomaria
            // las del momento y en base de datos siempre se guarda en 0
            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            var myReservation = (from a in db.reservation
                                 join u in db.users on a.id_users equals u.id_users
                                 join b in db.pitch on a.id_pitch equals b.id_pitch
                                 join c in db.scheduler on a.id_scheduler equals c.id_schedule
                                 where a.id_place == id_place && a.status == 1 && a.id_users_type == 3 && a.date >= currentDate
                                 orderby a.date ascending
                                 select new
                                 {
                                     name_user = u.name,
                                     id_user = u.id_users,
                                     phone_user = u.phone,
                                     hour = a.hour,
                                     date = a.date,
                                     value = (a.value == null ? c.value : a.value), // Se valida ya que si el cliente cambia el precio no se toma de Schedule
                                     status = a.status,
                                     id_reservation = a.id_reservation,
                                     pitch_description = b.description
                                 });

            return Ok(myReservation);
        }

        [ResponseType(typeof(reservation))]
        [Route("cancel/{id}"), HttpPost]
        public IHttpActionResult cancelReservation(int id)
        {
            reservation reservatons = (from a in db.reservation
                                       where a.id_reservation == id && (a.status == 1 || a.status == 2)
                                       select a).FirstOrDefault();

            if (reservatons != null)
            {
                reservatons.status = 3;
                db.SaveChanges();

                var reservationMultiple = from a in db.reservation_multipitch
                                          where a.id_reservation == reservatons.id_reservation
                                          select a;

                foreach (reservation_multipitch a in reservationMultiple)
                {
                    a.status = reservatons.status;
                }
                db.SaveChanges();
            }
            else
            { return BadRequest(); }

            return Ok();
        }

        [ResponseType(typeof(reservation))]
        [Route("cancelreservation/{place}/{lastDay}"), HttpGet]
        public IHttpActionResult GetcancelReservation(int place, int lastDay)
        {
            DateTime lastDays = DateTime.Now.AddDays(-lastDay);
            var cancelReservations = from a in db.reservation
                                     join b in db.users on a.id_users equals b.id_users into usersOnline
                                     join c in db.users_offline on a.id_users_offline equals c.id_users_offline into usersOffline
                                     join d in db.pitch on a.id_pitch equals d.id_pitch
                                     from uo in usersOnline.DefaultIfEmpty()
                                     from uof in usersOffline.DefaultIfEmpty()
                                     where (a.status == 3 || a.status == 4) &&
                                            a.id_place == place &&
                                            a.date > lastDays && a.date <= DateTime.Now
                                     select new
                                     {
                                         name = (uo.name == null) ? uof.name : uo.name,
                                         phone = (uo.phone == null) ? uof.phone : uo.phone,
                                         hour = a.hour,
                                         date = a.date,
                                         value = a.value,
                                         pitch = d.description
                                     };

            if (cancelReservations == null)
            {
                return NotFound();
            }

            return Ok(cancelReservations);
        }

        [ResponseType(typeof(reservation))]
        [Route("busy/multiple"), HttpPost]
        public IHttpActionResult Getreservationmultiple(reservation reservation)
        {
            IQueryable<object> lstReservation = null;
            IQueryable<object> lstReservation_multipitch = null;
            List<object> lstResult = new List<object>();
            TimeSpan timeNow = cf.GetDate().TimeOfDay;

            if (DateTime.Compare(reservation.date.Date, cf.GetDate().Date) == 0)
            {
                lstReservation = from a in db.reservation
                                 where a.id_place == reservation.id_place &&
                                 a.date.Equals(reservation.date) &&
                                 (TimeSpan.Compare(a.hour, timeNow)) == 1 &&
                                 a.status != 3 && a.status != 4
                                 select new { id_pitch = a.id_pitch, hour = a.hour };

                lstReservation_multipitch = from a in db.reservation_multipitch
                                            where a.id_place == reservation.id_place &&
                                            a.date.Equals(reservation.date) &&
                                            (TimeSpan.Compare(a.hour, timeNow)) == 1 &&
                                            a.status != 3 && a.status != 4 &&
                                            !a.type_pitch_insert.Equals("M")
                                            select new { id_pitch = a.id_pitch_multiple, hour = a.hour };

                lstResult.AddRange(lstReservation.ToList());
                lstResult.AddRange(lstReservation_multipitch.ToList());
            }
            else if (DateTime.Compare(reservation.date.Date, cf.GetDate().Date) == 1)
            {
                lstReservation = from a in db.reservation
                                 where a.id_place == reservation.id_place &&
                                 a.date.Equals(reservation.date) &&
                                 a.status != 3 && a.status != 4
                                 select new { id_pitch = a.id_pitch, hour = a.hour };

                lstReservation_multipitch = from a in db.reservation_multipitch
                                            where a.id_place == reservation.id_place &&
                                            a.date.Equals(reservation.date) &&
                                            a.status != 3 && a.status != 4 &&
                                            !a.type_pitch_insert.Equals("M")
                                            select new { id_pitch = a.id_pitch_multiple, hour = a.hour };

                lstResult.AddRange(lstReservation.ToList());
                lstResult.AddRange(lstReservation_multipitch.ToList());
            }

            return Ok(lstResult);
        }

        [ResponseType(typeof(reservation))]
        [Route("busy/single"), HttpPost]
        public IHttpActionResult Getreservationsingle(reservation reservation)
        {
            IQueryable<object> lstReservation = null;
            IQueryable<object> lstReservation_multipitch = null;
            List<object> lstResult = new List<object>();
            TimeSpan timeNow = cf.GetDate().TimeOfDay;

            if (DateTime.Compare(reservation.date.Date, cf.GetDate().Date) == 0)
            {
                lstReservation = from a in db.reservation
                                 where a.id_place == reservation.id_place &&
                                 a.date.Equals(reservation.date) &&
                                 (TimeSpan.Compare(a.hour, timeNow)) == 1 &&
                                 a.status != 3 && a.status != 4
                                 select new { id_pitch = a.id_pitch, hour = a.hour };

                lstReservation_multipitch = from a in db.reservation_multipitch
                                            where a.id_place == reservation.id_place &&
                                            a.date.Equals(reservation.date) &&
                                            (TimeSpan.Compare(a.hour, timeNow)) == 1 &&
                                            a.status != 3 && a.status != 4 &&
                                            !a.type_pitch_insert.Equals("S")
                                            select new { id_pitch = a.id_pitch_single, hour = a.hour };

                lstResult.AddRange(lstReservation.ToList());
                lstResult.AddRange(lstReservation_multipitch.ToList());
            }
            else if (DateTime.Compare(reservation.date.Date, cf.GetDate().Date) == 1)
            {
                lstReservation = from a in db.reservation
                                 where a.id_place == reservation.id_place &&
                                 a.date.Equals(reservation.date) &&
                                 a.status != 3 && a.status != 4
                                 select new { id_pitch = a.id_pitch, hour = a.hour };

                lstReservation_multipitch = from a in db.reservation_multipitch
                                            where a.id_place == reservation.id_place &&
                                            a.date.Equals(reservation.date) &&
                                            a.status != 3 && a.status != 4 &&
                                            !a.type_pitch_insert.Equals("S")
                                            select new { id_pitch = a.id_pitch_single, hour = a.hour };

                lstResult.AddRange(lstReservation.ToList());
                lstResult.AddRange(lstReservation_multipitch.ToList());
            }

            return Ok(lstResult);
        }

        [ResponseType(typeof(reservation))]
        [Route("customer/busy/multiple"), HttpPost]
        public IHttpActionResult GetReservationBusyCustomerMultiple(reservation reservation)
        {
            IQueryable<object> lstReservation = null;
            IQueryable<object> lstReservation_multipitch = null;
            List<object> lstResult = new List<object>();

            lstReservation = from a in db.reservation
                             join b in db.pitch on a.id_pitch equals b.id_pitch
                             where a.id_place == reservation.id_place &&
                             a.date.Equals(reservation.date) &&
                             a.status != 3 && a.status != 4 &&
                             b.id_pitch_type == 2
                             select new
                             {
                                 id_pitch = a.id_pitch,
                                 hour = a.hour,
                                 status = a.status,
                                 id_reservation = a.id_reservation,
                                 source = 1,
                                 name = (from user in db.users
                                         where user.id_users == a.id_users
                                         select user.name).FirstOrDefault() == null ? (from user in db.users_offline
                                                                                       where user.id_users_offline == a.id_users_offline
                                                                                       select user.name).FirstOrDefault() : (from user in db.users
                                                                                                                             where user.id_users == a.id_users
                                                                                                                             select user.name).FirstOrDefault()
                             };

            lstReservation_multipitch = from a in db.reservation_multipitch
                                        join b in db.reservation on a.id_reservation equals b.id_reservation
                                        where a.id_place == reservation.id_place &&
                                        a.date.Equals(reservation.date) &&
                                        a.status != 3 && a.status != 4 &&
                                        !a.type_pitch_insert.Equals("M")
                                        select new
                                        {
                                            id_pitch = a.id_pitch_multiple,
                                            hour = a.hour,
                                            status = a.status,
                                            id_reservation = a.id_reservation,
                                            source = 2,
                                            name = (from user in db.users
                                                    where user.id_users == b.id_users
                                                    select user.name).FirstOrDefault() == null ? (from user in db.users_offline
                                                                                                  where user.id_users_offline == b.id_users_offline
                                                                                                  select user.name).FirstOrDefault() : (from user in db.users
                                                                                                                                        where user.id_users == b.id_users
                                                                                                                                        select user.name).FirstOrDefault()
                                        };

            lstResult.AddRange(lstReservation.ToList());
            lstResult.AddRange(lstReservation_multipitch.ToList());

            return Ok(lstResult);
        }

        [ResponseType(typeof(reservation))]
        [Route("customer/busy/single"), HttpPost]
        public IHttpActionResult GetReservationBusyCustomerSingle(reservation reservation)
        {
            IQueryable<object> lstReservation = null;
            IQueryable<object> lstReservation_multipitch = null;
            List<object> lstResult = new List<object>();

            lstReservation = from a in db.reservation
                             join b in db.pitch on a.id_pitch equals b.id_pitch
                             where a.id_place == reservation.id_place &&
                             a.date.Equals(reservation.date) &&
                             a.status != 3 && a.status != 4 &&
                             b.id_pitch_type == 1
                             select new
                             {
                                 id_pitch = a.id_pitch,
                                 hour = a.hour,
                                 status = a.status,
                                 id_reservation = a.id_reservation,
                                 source = 1,
                                 name = (from user in db.users
                                         where user.id_users == a.id_users
                                         select user.name).FirstOrDefault() == null ? (from user in db.users_offline
                                                                                       where user.id_users_offline == a.id_users_offline
                                                                                       select user.name).FirstOrDefault() : (from user in db.users
                                                                                                                             where user.id_users == a.id_users
                                                                                                                             select user.name).FirstOrDefault()
                             };

            lstReservation_multipitch = from a in db.reservation_multipitch
                                        join b in db.reservation on a.id_reservation equals b.id_reservation
                                        where a.id_place == reservation.id_place &&
                                        a.date.Equals(reservation.date) &&
                                        a.status != 3 && a.status != 4 &&
                                        !a.type_pitch_insert.Equals("S")
                                        select new
                                        {
                                            id_pitch = a.id_pitch_single,
                                            hour = a.hour,
                                            status = a.status,
                                            id_reservation = a.id_reservation,
                                            source = 2,
                                            name = (from user in db.users
                                                    where user.id_users == b.id_users
                                                    select user.name).FirstOrDefault() == null ? (from user in db.users_offline
                                                                                                  where user.id_users_offline == b.id_users_offline
                                                                                                  select user.name).FirstOrDefault() : (from user in db.users
                                                                                                                                        where user.id_users == b.id_users
                                                                                                                                        select user.name).FirstOrDefault()
                                        };

            lstResult.AddRange(lstReservation.ToList());
            lstResult.AddRange(lstReservation_multipitch.ToList());

            return Ok(lstResult);
        }

        [ResponseType(typeof(reservation))]
        [Route("users/single"), HttpPost]
        public IHttpActionResult GetReservationUsersSingle(reservation reservation)
        {
            IQueryable<object> lstReservation = null;
            List<object> lstResult = new List<object>();

            if (DateTime.Compare(reservation.date.Date, cf.GetDate().Date) >= 0)
            {
                lstReservation = from a in db.reservation
                                 join b in db.pitch on a.id_pitch equals b.id_pitch
                                 where a.id_place == reservation.id_place &&
                                 a.date.Equals(reservation.date) &&
                                 a.status == 1 &&
                                 a.id_users != null &&
                                 b.id_pitch_type == 1
                                 select new { id_pitch = a.id_pitch, hour = a.hour, status = a.status, id_reservation = a.id_reservation, name = b.description, date = a.date };

                lstResult.AddRange(lstReservation.ToList());
            }

            return Ok(lstResult);
        }

        [ResponseType(typeof(reservation))]
        [Route("users/multiple"), HttpPost]
        public IHttpActionResult GetReservationUsersMultiple(reservation reservation)
        {
            IQueryable<object> lstReservation = null;
            List<object> lstResult = new List<object>();

            if (DateTime.Compare(reservation.date.Date, cf.GetDate().Date) >= 0)
            {
                lstReservation = from a in db.reservation
                                 join b in db.pitch on a.id_pitch equals b.id_pitch
                                 where a.id_place == reservation.id_place &&
                                 a.date.Equals(reservation.date) &&
                                 a.status == 1 &&
                                 a.id_users != null &&
                                 b.id_pitch_type == 2
                                 select new { id_pitch = a.id_pitch, hour = a.hour, status = a.status, id_reservation = a.id_reservation, name = b.description, date = a.date };

                lstResult.AddRange(lstReservation.ToList());
            }
            return Ok(lstResult);
        }

        [ResponseType(typeof(reservation))]
        [Route("panelweb"), HttpPost]
        public IHttpActionResult GetPanelWeb(reservation reservation)
        {
            DateTime dt = DateTime.Now;
            var info = from a in db.pitch
                       where a.id_place == 1
                       select a.id_pitch;

            var dataPlace = from a in db.opening
                            where a.id_place == 1
                            select new { a.hour_start, a.hour_end };

            foreach (var a in dataPlace)
            {
                var tiempoTotal = a.hour_end - a.hour_start;
            }

            return Ok();
        }

        // PUT: api/reservations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putreservation(int id, reservation reservation)
        {
            // Validamos si la reserva donde queremos modificar existe
            reservation findReservation = (from a in db.reservation
                                           where a.id_reservation == id &&
                                           a.status == 1
                                           select a).FirstOrDefault();

            if (findReservation == null)
            { return NotFound(); }

            DateTime dateEntry = reservation.date.Date;
            DateTime holiday = (from a in db.holidays
                                where a.date == dateEntry
                                select a.date).FirstOrDefault();

            if (holiday != null)
            {
                int? scheduler = (from a in db.scheduler
                                  where a.id_day == 8 &&
                                        a.id_pitch == reservation.id_pitch &&
                                        a.hour.Hours == reservation.hour.Hours
                                  select a.id_schedule).FirstOrDefault();

                if (scheduler != null)
                { reservation.id_scheduler = (int)scheduler; }
            }
            else
            {
                int day = (int)reservation.date.DayOfWeek;
                if (day == 0) { day = 7; }

                int? scheduler = (from a in db.scheduler
                                  where a.id_day == day &&
                                        a.id_pitch == reservation.id_pitch &&
                                        a.hour.Hours == reservation.hour.Hours
                                  select a.id_schedule).FirstOrDefault();

                if (scheduler != null)
                { reservation.id_scheduler = (int)scheduler; }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            reservation.id_reservation = id;
            reservation.date_insert = cf.GetDate();
            reservation.status = findReservation.status;
            db.Entry(reservation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!reservationExists(id))
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

        // POST: api/reservations
        [ResponseType(typeof(reservation))]
        [Route("multiple"), HttpPost]
        public async Task<IHttpActionResult> Postreservation(reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Fechas de la reserva
            DateTime dtNow = cf.GetDate().Date;
            DateTime dateEntry = reservation.date.Date;
            int id_pithc = reservation.id_pitch;

            // Revisamos si la fecha enviada es menos de la fecha actual
            int result = DateTime.Compare(dateEntry, dtNow);
            if (result < 0)
            { return Conflict(); }

            // Buscar si ya existe una reservacion para ese horario
            List<reservation> lstReservation = await (from a in db.reservation
                                                      where a.date == dateEntry &&
                                                      (a.status == 1 || a.status == 2) &&
                                                      a.id_pitch == reservation.id_pitch &&
                                                      a.hour == reservation.hour
                                                      select a).ToListAsync();
            if (lstReservation.Count > 0)
            { return Conflict(); }

            // Numero del dia de la semana
            int intDayOfWeek = reservation.date.DayOfWeek.GetHashCode();
            if (intDayOfWeek == 0) { intDayOfWeek = 7; }

            // Buscar programacion de la cancha
            var schedule = await (from a in db.scheduler
                                  where a.id_day == intDayOfWeek &&
                                        a.id_pitch == reservation.id_pitch &&
                                        a.hour.Hours == reservation.hour.Hours
                                  select new { Id = a.id_schedule, Value = a.value }).FirstOrDefaultAsync();

            // Confirmamos si el Place tiene confirmacion automatica
            bool? autoConfirm = await (from a in db.place
                                       where a.id_place == reservation.id_place
                                       select a.autoconfirm).FirstOrDefaultAsync();

            if (schedule != null)
            {
                reservation.id_scheduler = schedule.Id;
                reservation.date_insert = cf.GetDate();
                reservation.value = schedule.Value;
                reservation.id_users_type = 3;

                if (autoConfirm != null && (bool)autoConfirm)
                    reservation.status = 2;
                else
                    reservation.status = 1;
            }
            else
            { return BadRequest("No se encontro programacion para la cancha seleccionada"); }

            db.reservation.Add(reservation);
            await db.SaveChangesAsync();

            #region Validar cancha multiple

            // Busca las canchas hijas
            var findMultiPitch = (from a in db.multi_pitch
                                  where a.id_pitch_multiple == reservation.id_pitch
                                  select a.id_pitch_single).ToList();

            foreach (var single in findMultiPitch)
            {
                reservation_multipitch newRow = new reservation_multipitch();
                newRow.id_pitch_single = single;
                newRow.id_place = reservation.id_place;
                newRow.id_reservation = reservation.id_reservation;
                newRow.id_pitch_multiple = reservation.id_pitch;
                newRow.status = reservation.status;
                newRow.hour = reservation.hour;
                newRow.date = reservation.date;
                newRow.type_pitch_insert = "M";
                db.reservation_multipitch.Add(newRow);
            }
            db.SaveChanges();

            // Busca las canchas padres 
            var findMultiPitchFather = (from a in db.multi_pitch
                                        where a.id_pitch_single == reservation.id_pitch
                                        select a.id_pitch_multiple).ToList();

            foreach (var pitchMulti in findMultiPitchFather)
            {
                reservation_multipitch newRow = new reservation_multipitch();
                newRow.id_pitch_single = reservation.id_pitch;
                newRow.id_place = reservation.id_place;
                newRow.id_reservation = reservation.id_reservation;
                newRow.id_pitch_multiple = pitchMulti;
                newRow.status = reservation.status;
                newRow.hour = reservation.hour;
                newRow.date = reservation.date;
                newRow.type_pitch_insert = "S";
                db.reservation_multipitch.Add(newRow);
            }
            db.SaveChanges();

            #endregion

            return Ok(reservation);
        }

        // POST: api/reservations
        [ResponseType(typeof(reservation))]
        [Route("single"), HttpPost]
        public async Task<IHttpActionResult> PostreservationSingle(reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Fechas de la reserva
            DateTime dtNow = cf.GetDate().Date;
            DateTime dateEntry = reservation.date.Date;
            int id_pithc = reservation.id_pitch;

            // Revisamos si la fecha enviada es menos de la fecha actual
            int result = DateTime.Compare(dateEntry, dtNow);
            if (result < 0)
            { return Conflict(); }

            // Buscar si ya existe una reservacion para ese horario
            List<reservation> lstReservation = await (from a in db.reservation
                                                      where a.date == dateEntry &&
                                                      (a.status == 1 || a.status == 2) &&
                                                      a.id_pitch == reservation.id_pitch &&
                                                      a.hour == reservation.hour
                                                      select a).ToListAsync();
            if (lstReservation.Count > 0)
            { return Conflict(); }

            // Numero del dia de la semana
            int intDayOfWeek = reservation.date.DayOfWeek.GetHashCode();
            if (intDayOfWeek == 0) { intDayOfWeek = 7; }

            // Buscar programacion de la cancha
            var schedule = await (from a in db.scheduler
                                  where a.id_day == intDayOfWeek &&
                                        a.id_pitch == reservation.id_pitch &&
                                        a.hour.Hours == reservation.hour.Hours
                                  select new { Id = a.id_schedule, Value = a.value }).FirstOrDefaultAsync();

            // Confirmamos si el Place tiene confirmacion automatica
            bool? autoConfirm = await (from a in db.place
                                       where a.id_place == reservation.id_place
                                       select a.autoconfirm).FirstOrDefaultAsync();

            if (schedule != null)
            {
                reservation.id_scheduler = schedule.Id;
                reservation.date_insert = cf.GetDate();
                reservation.value = schedule.Value;
                reservation.id_users_type = 3;

                if (autoConfirm != null && (bool)autoConfirm)
                    reservation.status = 2;
                else
                    reservation.status = 1;
            }
            else
            { return BadRequest("No se encontro programacion para la cancha seleccionada"); }

            db.reservation.Add(reservation);
            await db.SaveChangesAsync();

            #region Validar cancha multiple

            var findMultiPitch = (from a in db.multi_pitch
                                  where a.id_pitch_single == reservation.id_pitch
                                  select a.id_pitch_multiple).ToList();

            foreach (var pitchMulti in findMultiPitch)
            {
                reservation_multipitch newRow = new reservation_multipitch();
                newRow.id_pitch_single = reservation.id_pitch;
                newRow.id_place = reservation.id_place;
                newRow.id_reservation = reservation.id_reservation;
                newRow.id_pitch_multiple = pitchMulti;
                newRow.status = reservation.status;
                newRow.hour = reservation.hour;
                newRow.date = reservation.date;
                newRow.type_pitch_insert = "S";
                db.reservation_multipitch.Add(newRow);
            }

            await db.SaveChangesAsync();
            #endregion

            return Ok(reservation);
        }

        // POST: api/reservations
        [ResponseType(typeof(reservation))]
        [Route("customer/single"), HttpPost]
        public async Task<IHttpActionResult> PostreservationCustomerSingle(reservation reservation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                CommonFunctions.Log(reservation);

                // Fechas de la reserva
                DateTime dtNow = cf.GetDate().Date;
                DateTime dateEntry = reservation.date.Date;
                int id_pithc = reservation.id_pitch;

                // Revisamos si la fecha enviada es menos de la fecha actual
                int result = DateTime.Compare(dateEntry, dtNow);
                if (result < 0)
                {
                    CommonFunctions.Log($"Conflicto de fechas: {dateEntry} - Fecha Actual {dtNow}");
                    return Conflict();
                }

                // Buscar si ya existe una reservacion para ese horario
                List<reservation> lstReservation = await (from a in db.reservation
                                                          where a.date == dateEntry &&
                                                          (a.status == 1 || a.status == 2) &&
                                                          a.id_pitch == reservation.id_pitch &&
                                                          a.hour == reservation.hour
                                                          select a).ToListAsync();
                if (lstReservation.Count > 0)
                {
                    CommonFunctions.Log($"No se pudo hacer la reserva porque ya esta creada");
                    return Conflict();
                }

                // Numero del dia de la semana
                int intDayOfWeek = reservation.date.DayOfWeek.GetHashCode();
                if (intDayOfWeek == 0) { intDayOfWeek = 7; }

                // Buscar programacion de la cancha
                var schedule = await (from a in db.scheduler
                                      where a.id_day == intDayOfWeek &&
                                            a.id_pitch == reservation.id_pitch &&
                                            a.hour.Hours == reservation.hour.Hours
                                      select new { Id = a.id_schedule, Value = a.value }).FirstOrDefaultAsync();


                // Confirmamos si el Place tiene confirmacion automatica
                bool? autoConfirm = await (from a in db.place
                                           where a.id_place == reservation.id_place
                                           select a.autoconfirm).FirstOrDefaultAsync();

                if (schedule != null)
                {
                    reservation.id_scheduler = schedule.Id;
                    reservation.date_insert = cf.GetDate();
                    if (reservation.value == null) { reservation.value = schedule.Value; }
                    reservation.id_users_type = 1;

                    if (autoConfirm != null && (bool)autoConfirm)
                        reservation.status = 2;
                    else
                        reservation.status = 1;
                }
                else
                {
                    CommonFunctions.Log($"No se encontro programacion para la cancha seleccionada");
                    return BadRequest("No se encontro programacion para la cancha seleccionada");
                }

                db.reservation.Add(reservation);
                await db.SaveChangesAsync();

                #region Validar cancha multiple

                var findMultiPitch = (from a in db.multi_pitch
                                      where a.id_pitch_single == reservation.id_pitch
                                      select a.id_pitch_multiple).ToList();

                foreach (var pitchMulti in findMultiPitch)
                {
                    reservation_multipitch newRow = new reservation_multipitch();
                    newRow.id_pitch_single = reservation.id_pitch;
                    newRow.id_place = reservation.id_place;
                    newRow.id_reservation = reservation.id_reservation;
                    newRow.id_pitch_multiple = pitchMulti;
                    newRow.status = reservation.status;
                    newRow.hour = reservation.hour;
                    newRow.date = reservation.date;
                    newRow.type_pitch_insert = "S";
                    db.reservation_multipitch.Add(newRow);
                }

                await db.SaveChangesAsync();
                #endregion

                CommonFunctions.Log($"Termino OK");
                return Ok(reservation.id_reservation);
            }
            catch (Exception e)
            {
                CommonFunctions.Log($"Excepcion {e.ToString()}");
                return BadRequest("Se presento un error al hacer la reserva");
            }
        }

        // POST: api/reservations
        [ResponseType(typeof(reservation))]
        [Route("customer/multiple"), HttpPost]
        public async Task<IHttpActionResult> PostreservationCustomerMultiple(reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Fechas de la reserva
            DateTime dtNow = cf.GetDate().Date;
            DateTime dateEntry = reservation.date.Date;
            int id_pithc = reservation.id_pitch;

            // Revisamos si la fecha enviada es menos de la fecha actual
            int result = DateTime.Compare(dateEntry, dtNow);
            if (result < 0)
            { return Conflict(); }

            // Buscar si ya existe una reservacion para ese horario
            List<reservation> lstReservation = await (from a in db.reservation
                                                      where a.date == dateEntry &&
                                                      (a.status == 1 || a.status == 2) &&
                                                      a.id_pitch == reservation.id_pitch &&
                                                      a.hour == reservation.hour
                                                      select a).ToListAsync();
            if (lstReservation.Count > 0)
            { return Conflict(); }

            // Numero del dia de la semana
            int intDayOfWeek = reservation.date.DayOfWeek.GetHashCode();
            if (intDayOfWeek == 0) { intDayOfWeek = 7; }

            // Buscar programacion de la cancha
            var schedule = await (from a in db.scheduler
                                  where a.id_day == intDayOfWeek &&
                                        a.id_pitch == reservation.id_pitch &&
                                        a.hour.Hours == reservation.hour.Hours
                                  select new { Id = a.id_schedule, Value = a.value }).FirstOrDefaultAsync();

            // Confirmamos si el Place tiene confirmacion automatica
            bool? autoConfirm = await (from a in db.place
                                       where a.id_place == reservation.id_place
                                       select a.autoconfirm).FirstOrDefaultAsync();

            if (schedule != null)
            {
                reservation.id_scheduler = schedule.Id;
                reservation.date_insert = cf.GetDate();
                if (reservation.value == null) { reservation.value = schedule.Value; }
                reservation.id_users_type = 1;

                if (autoConfirm != null && (bool)autoConfirm)
                    reservation.status = 2;
                else
                    reservation.status = 1;
            }
            else
            { return BadRequest("No se encontro programacion para la cancha seleccionada"); }

            db.reservation.Add(reservation);
            await db.SaveChangesAsync();

            #region Validar cancha multiple

            var findMultiPitch = (from a in db.multi_pitch
                                  where a.id_pitch_multiple == reservation.id_pitch
                                  select a.id_pitch_single).ToList();

            foreach (var single in findMultiPitch)
            {
                reservation_multipitch newRow = new reservation_multipitch();
                newRow.id_pitch_single = single;
                newRow.id_place = reservation.id_place;
                newRow.id_reservation = reservation.id_reservation;
                newRow.id_pitch_multiple = reservation.id_pitch;
                newRow.status = reservation.status;
                newRow.hour = reservation.hour;
                newRow.date = reservation.date;
                newRow.type_pitch_insert = "M";
                db.reservation_multipitch.Add(newRow);
            }
            db.SaveChanges();

            var findMultiPitchFather = (from a in db.multi_pitch
                                        where a.id_pitch_single == reservation.id_pitch
                                        select a.id_pitch_multiple).ToList();

            foreach (var pitchMulti in findMultiPitchFather)
            {
                reservation_multipitch newRow = new reservation_multipitch();
                newRow.id_pitch_single = reservation.id_pitch;
                newRow.id_place = reservation.id_place;
                newRow.id_reservation = reservation.id_reservation;
                newRow.id_pitch_multiple = pitchMulti;
                newRow.status = reservation.status;
                newRow.hour = reservation.hour;
                newRow.date = reservation.date;
                newRow.type_pitch_insert = "S";
                db.reservation_multipitch.Add(newRow);
            }
            db.SaveChanges();


            #endregion

            return Ok(reservation.id_reservation);
        }

        // GET: api/reservations
        [ResponseType(typeof(reservation))]
        [Route("customer/pending"), HttpPost]
        public IHttpActionResult GetReservationLstPending(reservation reservation)
        {
            var date = DateTime.Now.Date;
            var lstReservation = from a in db.reservation
                                 where a.status == 1 &&
                                 a.id_place == reservation.id_place &&
                                 a.date >= date
                                 select new { date = a.date, id_pitch = a.id_pitch, hour = a.hour, status = a.status, id_reservation = a.id_reservation };

            if (lstReservation.ToList().Count == 0)
            { return Conflict(); }

            return Ok(lstReservation);
        }

        // POST: api/reservations
        [Route("changestatus"), HttpPost]
        public async Task<IHttpActionResult> PostChangeStatus(reservation reservation)
        {
            // Buscamos el registro a modificar
            var findReservation = await (from a in db.reservation
                                         where a.id_reservation == reservation.id_reservation
                                         select a).FirstOrDefaultAsync();

            // Validamos si el codigo del estado enviado es valido
            var findStatus = from a in db.status_type
                             where a.id_status == reservation.status
                             select a;

            if (findStatus != null && findReservation != null)
            {
                findReservation.status = reservation.status;
                await db.SaveChangesAsync();

                var findReservationMultiple = from a in db.reservation_multipitch
                                              where a.id_reservation == reservation.id_reservation
                                              select a;

                foreach (var status in findReservationMultiple)
                {
                    status.status = reservation.status;
                }

                await db.SaveChangesAsync();
            }
            else
            { return BadRequest(); }

            return Ok();
        }

        // POST: api/reservations
        [Route("updateReservation"), HttpPost]
        public async Task<IHttpActionResult> PostUpdateReservation(reservation reservation)
        {
            // Buscamos el registro a modificar
            var findReservation = await (from a in db.reservation
                                         where a.id_reservation == reservation.id_reservation
                                         select a).FirstOrDefaultAsync();

            // Validamos si el codigo del estado enviado es valido
            var findStatus = from a in db.status_type
                             where a.id_status == reservation.status
                             select a;

            if (findStatus != null && findReservation != null)
            {
                findReservation.status = reservation.status;
                await db.SaveChangesAsync();

                var findReservationMultiple = from a in db.reservation_multipitch
                                              where a.id_reservation == reservation.id_reservation
                                              select a;

                foreach (var status in findReservationMultiple)
                {
                    status.status = reservation.status;
                }

                if (reservation.description != null && !reservation.description.Trim().Equals(string.Empty)) findReservation.description = reservation.description.Trim();
                if (findReservation.value != null) findReservation.value = reservation.value;

                await db.SaveChangesAsync();
            }
            else
            { return BadRequest(); }

            return Ok();
        }

        // GET: api/reservations/findfreepitchbyplace
        [Route("findfreepitchbyplace"), HttpPost]
        public IHttpActionResult GetFreePitchByPlace(reservation reservation)
        {
            int day = (int)reservation.date.DayOfWeek;
            if (day == 0) { day = 7; }

            List<int> lstReservationsPlace = new List<int>();

            #region Reservas de canchas simples
            List<reservation> lstReservationsSinglePlace = (from r in db.reservation
                                                            where r.id_place.Equals(reservation.id_place) &&
                                                                  r.date.Equals(reservation.date) &&
                                                                  r.hour.Equals(reservation.hour) &&
                                                                  !r.status.Equals(3) &&
                                                                  !r.status.Equals(4)
                                                            select r).ToList();

            lstReservationsSinglePlace.ForEach(r => lstReservationsPlace.Add(r.id_scheduler));


            #endregion

            #region Reservas multiples

            List<int> lstIdReservationAux = new List<int>();
            lstReservationsSinglePlace.ForEach(r => lstIdReservationAux.Add(r.id_reservation));

            List<reservation_multipitch> lstReservationsMultiplePlace = (from rm in db.reservation_multipitch
                                                                         where lstIdReservationAux.Contains(rm.id_reservation.Value)
                                                                         select rm).ToList();

            List<int> lstPitchs = new List<int>();
            lstReservationsMultiplePlace.ForEach(rm => lstPitchs.Add(rm.type_pitch_insert.Equals("S") ? rm.id_pitch_multiple.Value : rm.id_pitch_single.Value));

            #endregion

            if (lstReservationsPlace == null) lstReservationsPlace = new List<int>();

            List<scheduler> lstScheduler = (from a in db.scheduler
                                            join b in db.pitch on a.id_pitch equals b.id_pitch
                                            where !lstReservationsPlace.Contains(a.id_schedule) &&
                                                   !lstPitchs.Contains(b.id_pitch) &&
                                                  a.id_day == day &&
                                                  b.id_place == reservation.id_place &&
                                                  a.hour == reservation.hour
                                            select a).ToList();

            var getPitch = (from a in lstScheduler
                            join pt in db.pitch on a.id_pitch equals pt.id_pitch
                            join pl in db.place on pt.id_place equals pl.id_place
                            select new { pt.id_place, pl.name, pl.address, reservation.date, reservation.hour, pt.id_pitch, pt.description, pt.id_pitch_type, a.value }).OrderBy(x => Guid.NewGuid());

            return Ok(getPitch);
        }

        // GET: api/reservations/findfreepitch
        [Route("findfreepitch"), HttpPost]
        public IHttpActionResult GetFreePitch(reservation reservation)
        {
            int day = (int)reservation.date.DayOfWeek;
            if (day == 0) { day = 7; }

            List<int> lstReservationsPlace = new List<int>();

            #region Reservas de canchas simples
            List<reservation> lstReservationsSinglePlace = (from r in db.reservation
                                                            where r.date.Equals(reservation.date) &&
                                                                  r.hour.Equals(reservation.hour) &&
                                                                  !r.status.Equals(3) &&
                                                                  !r.status.Equals(4)
                                                            select r).ToList();

            lstReservationsSinglePlace.ForEach(r => lstReservationsPlace.Add(r.id_scheduler));


            #endregion

            #region Reservas multiples

            List<int> lstIdReservationAux = new List<int>();
            lstReservationsSinglePlace.ForEach(r => lstIdReservationAux.Add(r.id_reservation));

            List<reservation_multipitch> lstReservationsMultiplePlace = (from rm in db.reservation_multipitch
                                                                         where lstIdReservationAux.Contains(rm.id_reservation.Value)
                                                                         select rm).ToList();

            List<int> lstPitchs = new List<int>();
            lstReservationsMultiplePlace.ForEach(rm => lstPitchs.Add(rm.type_pitch_insert.Equals("S") ? rm.id_pitch_multiple.Value : rm.id_pitch_single.Value));

            #endregion

            if (lstReservationsPlace == null) lstReservationsPlace = new List<int>();

            List<scheduler> lstScheduler = (from a in db.scheduler
                                            join b in db.pitch on a.id_pitch equals b.id_pitch
                                            join p in db.place on b.id_place equals p.id_place
                                            where !lstReservationsPlace.Contains(a.id_schedule) &&
                                                   !lstPitchs.Contains(b.id_pitch) &&
                                                  a.id_day == day &&
                                                  a.hour == reservation.hour &&
                                                  p.status == true
                                            select a).ToList();

            var getPitch = (from a in lstScheduler
                            join pt in db.pitch on a.id_pitch equals pt.id_pitch
                            join pl in db.place on pt.id_place equals pl.id_place
                            select new { pt.id_place, pl.name, pl.address, pl.profile_img, reservation.date, reservation.hour, pt.id_pitch, pt.description, pt.id_pitch_type, a.value }).OrderBy(x => Guid.NewGuid());

            return Ok(getPitch);
        }

        // DELETE: api/reservations/5
        [ResponseType(typeof(reservation))]
        public IHttpActionResult Deletereservation(int id)
        {
            reservation reservation = db.reservation.Find(id);
            if (reservation == null)
            {
                return NotFound();
            }

            db.reservation.Remove(reservation);
            db.SaveChanges();

            return Ok(reservation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool reservationExists(int id)
        {
            return db.reservation.Count(e => e.id_reservation == id) > 0;
        }
    }
}