using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;

namespace ImageSharing.Business.Tools
{
    public class Utility
    {

        public static string GetHashString(string password)
        {
            //переводим строку в байт-массим  
            byte[] bytes = Encoding.Unicode.GetBytes(password);

            //создаем объект для получения средст шифрования  
            MD5CryptoServiceProvider CSP =
                new MD5CryptoServiceProvider();

            //вычисляем хеш-представление в байтах  
            byte[] byteHash = CSP.ComputeHash(bytes);

            string hash = string.Empty;

            //формируем одну цельную строку из массива  
            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);

            return new Guid(hash).ToString();
        }

        private static string MAIL_ADDRESS = "ImageSharingApp@yandex.ru";
        private static string MAIL_LOGIN = "ImageSharingApp";
        private static string MAIL_PASSWORD = "ImageSharingApppass";
        private static string MAIN_PASS_TO_APP = "localhost:52179";

        public static void ConfirmRegistration(string to, Guid key)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(MAIL_ADDRESS);
            mail.To.Add(new MailAddress(to));
            mail.Subject = "Confirma registration";

            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Body = "<a href=http://" + MAIN_PASS_TO_APP +"/User/ConfirmRegistration?key=" + key.ToString() + ">Confirm</a>";

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(mail.Body, null, "text/html");
            mail.AlternateViews.Add(htmlView);

            SmtpClient client = new SmtpClient();
            client.Host = "smtp.yandex.ru";
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(MAIL_LOGIN, MAIL_PASSWORD);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(mail);
            mail.Dispose();
        }

        public static bool CheckName(string name)
        {

            int MinLength = 4;
            int MaxLength = 20;

            if (String.IsNullOrEmpty(name) && name.Length < MinLength && name.Length > MaxLength)
            {
                return false;
            }
            return true;
        }

        public static bool CheckPassword(string password)
        {
            int MinLength = 4;
            int MaxLength = 20;

            if (String.IsNullOrEmpty(password) && password.Length < MinLength && password.Length > MaxLength)
            {
                return false;
            }
            return true;
        }

        public static bool IsPasswordsEquals(string pass, string ConfPass)
        {
            return pass.Equals(ConfPass);
        }

        private bool invalid = false;

        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        public static bool IsRightRegistrationData(string firstName, string secondName, string password, string confirmPassword, string email)
        {
            Utility utility = new Utility();

            if (CheckName(firstName) && CheckName(secondName) && CheckPassword(password) && CheckPassword(confirmPassword) && IsPasswordsEquals(password, confirmPassword) && utility.IsValidEmail(email))
            {
                return true;
            }
            return false;
        }

        public static bool CheckTypePhotoDuringRegistration(string PhotoType)
        {
            string[] PhotoTypes = { "image/jpeg", "image/jpg", "image/png" };
            foreach (var type in PhotoTypes)
            {
                if (type.Equals(PhotoType))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
