using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ComponentModel;

namespace mvc4gw7.Models
{
    public class EmailServices
    {        
        public static void SendEmail(RegisterModel registerModel)
        {
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("amorbc@mail.ru", "mbcaor");
            client.Host = "smtp.mail.ru";
            client.Port = 25;
            client.EnableSsl = true;
            MailAddress from = new MailAddress("amorbc@mail.ru","ABC", System.Text.Encoding.UTF8);
            MailAddress to = new MailAddress(registerModel.Email);
            MailMessage message = new MailMessage();
            message.From = from;
            message.To.Add(to);
            message.Body = registerModel.Name+" "+ registerModel.Patronymic+", Вы зарегистрировались на сайте groundwork.somee.com компании Amor Broadcasting Company. Теперь Вы можете размещать свои фотографии, рисунки, графические работы на наших страницах, и так же как и мы делиться своими впечатлениями с теми, кто Вам дорог. НО! Необходимо сделать еще один шаг - подтвердить регистрацию. Для этого перейдите по ссылке http://www.groundwork.somee.com/Account/RegisterConfirm?username=" + registerModel.UserName + " Присоединяйтесь к сообществу! С уважением, ABC.";
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = "Вы зарегистрированны!";
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            client.Send(message);
            message.Dispose();
        }       
    }
}