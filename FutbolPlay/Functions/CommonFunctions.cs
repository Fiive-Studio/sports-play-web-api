using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace FutbolPlay.Functions
{
    public class CommonFunctions
    {
        private Random numberRamdon = new Random();

        /// <summary>
        /// Metodo que valida un correo electronico
        /// </summary>
        /// <param name="email">Correo electronico a validar</param>
        /// <returns>True si es correcto, de los contrario False</returns>
        public bool ValidateEmail(string email)
        {
            const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

            return Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        /// <summary>
        /// Metodo que permite generar un nuevo password con encriptacion SHA1
        /// </summary>
        /// <returns>Nuevo password SHA1</returns>
        public string generateNewPassword(out string numberKeyGenerated)
        {
            string strNewPassword = string.Empty;
            UTF8Encoding enc = new UTF8Encoding();
            int numberKey = numberRamdon.Next(0, 999999);
            numberKeyGenerated = numberKey.ToString().PadLeft(6, '0');
            byte[] result;
            byte[] data = enc.GetBytes(numberKeyGenerated);
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(data);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                // Convertimos los valores en hexadecimal
                // cuando tiene una cifra hay que rellenarlo con cero
                // para que siempre ocupen dos dígitos.
                if (result[i] < 16)
                    sb.Append("0");
                sb.Append(result[i].ToString("x"));
            }
            strNewPassword = sb.ToString().ToUpper();

            return strNewPassword;
        }

        /// <summary>
        /// Obtener el SHA1 a partir de una cadena de texto
        /// </summary>
        /// <param name="password">Password ingresado</param>
        /// <returns>Cadena SHA1</returns>
        public string getSHA1(string password)
        {
            string strResult = string.Empty;
            UTF8Encoding enc = new UTF8Encoding();
            byte[] result;
            byte[] data = enc.GetBytes(password);
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(data);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                // Convertimos los valores en hexadecimal
                // cuando tiene una cifra hay que rellenarlo con cero
                // para que siempre ocupen dos dígitos.
                if (result[i] < 16)
                    sb.Append("0");
                sb.Append(result[i].ToString("x"));
            }
            strResult = sb.ToString().ToUpper();

            return strResult;
        }

        /// <summary>
        /// Metodo que envia el correo electronico al cliente cuando ha olvidado su contraseña
        /// </summary>
        /// <param name="user">Datos del usuarios</param>
        /// <param name="numberKeyGenerated">Nuevo password para enviarle al cliente</param>
        /// <returns></returns>
        public bool sendMail(users user, string numberKeyGenerated)
        {
            bool booSendMail = false;
            string body = string.Empty;
            try
            {
                using (StreamReader reader = new StreamReader(string.Format(@"{0}\Templates_Html\ForgotPasswordUser.html", System.AppDomain.CurrentDomain.BaseDirectory)))
                {
                    body = reader.ReadToEnd();
                }
                var fromAddress = new MailAddress("[CORREO_ELECTRONICO]", "[NOMBRE DE QUIEN ENVIA EL CORREO]");
                var toAddress = new MailAddress(user.email, user.name);
                const string fromPassword = "[CONTRASEÑA]";
                const string subject = "Se ha generado una contraseña temporal de Sports Play";
                body = string.Format(body, user.name, numberKeyGenerated);

                var smtp = new SmtpClient
                {
                    Host = "[SERVER DE CORREO SMTP]",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
                booSendMail = true;
                CommonFunctions.Log("Se envio el correo");
            }
            catch (Exception e)
            {
                booSendMail = false;
                CommonFunctions.Log(e.ToString());
            }
            return booSendMail;
        }

        /// <summary>
        /// Metodo que envia el correo electronico al cliente cuando ha olvidado su contraseña
        /// </summary>
        /// <param name="customer">Datos del usuarios</param>
        /// <param name="numberKeyGenerated">Nuevo password para enviarle al cliente</param>
        /// <returns></returns>
        public bool sendMail(customer customer, string numberKeyGenerated)
        {
            bool booSendMail = false;
            string body = string.Empty;
            try
            {
                using (StreamReader reader = new StreamReader(string.Format(@"{0}\Templates_Html\ForgotPasswordCoach.html", System.AppDomain.CurrentDomain.BaseDirectory)))
                {
                    body = reader.ReadToEnd();
                }
                var fromAddress = new MailAddress("[CORREO_ELECTRONICO]", "[NOMBRE DE QUIEN ENVIA EL CORREO]");
                var toAddress = new MailAddress(customer.email, customer.full_name);
                const string fromPassword = "[CONTRASEÑA]";
                const string subject = "Se ha generado una contraseña temporal de Sports Play";
                body = string.Format(body, customer.full_name, numberKeyGenerated);

                var smtp = new SmtpClient
                {
                    Host = "[SERVER DE CORREO SMTP]",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
                booSendMail = true;
            }
            catch (Exception exc)
            {
                booSendMail = false;
            }
            return booSendMail;
        }

        public DateTime GetDate()
        {
            DateTime dt = DateTime.Now;
            dt = dt.AddHours(-5);

            return dt;
        }

        public static void Log(string message)
        {
            string pathFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", $"logday{DateTime.Now.ToString("yyyyMMdd")}.txt");
            File.AppendAllText(pathFile, $"{DateTime.Now} - {message}{Environment.NewLine}");
        }

        public static void Log(reservation reservation)
        {
            string message = $"Id place: {reservation.id_place} - Id pitch: {reservation.id_pitch} - Value: {reservation.value} - Description: {reservation.description}";
            if (reservation.id_users.HasValue) { message += string.Concat(Environment.NewLine, "Id de usuario registrado:", reservation.id_users.Value); }
            if (reservation.id_users_offline.HasValue) { message += string.Concat(Environment.NewLine, "Id de usuario no registrado:", reservation.id_users_offline.Value); }

            Log(message);
        }
    }
}