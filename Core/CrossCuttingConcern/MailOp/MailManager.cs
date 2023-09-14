using System.Net;
using System.Net.Mail;
using Core.Const;

namespace Core.CrossCuttingConcern.MailOp
{
    public class MailManager
    {
        public static bool Send(string to, string title, string message)   //to kime gidecek title başlığı message içeriği olcak
        {
            MailMessage msg = new MailMessage(CoreKeys.EMAILADDRESS, to); //kimden kime gidecek mail -> 
            msg.Subject = title;
            msg.Body = message;
            msg.IsBodyHtml = true;  //body de html kullanılacak mı kullanılmayacak mı true yazarsak mesajda html kodları da kullanılabiliyor.

            // msg.Attachments maile kod ile dosya da ekletebiliyoruz burada  //mail.Attachments.Add(new Attachment("C:\\file.zip"));

            SmtpClient smtp = new SmtpClient();  //mail gönderirken smtp() ile çalışırken mail alma ise pop3 maillerin size gelip okunması biz mail göndereceğimizden smtp kullanacağız.
            smtp.Credentials = new NetworkCredential(CoreKeys.EMAILUSER, CoreKeys.EMAILPASSWORD);
            smtp.Host = "smtp-mail.outlook.com"; //smtpnin mail alma servisi.        // "mail.sinanarslan.com"; // mail.gmail.com       
            smtp.Port = 587;   //Eskiden 25 di virüs spam mail gibi şeylerden türkiyeden yapıldığından port değişince de ciddi bir azalma olduğu ön görüldüğünden ki olmuş değiştirilmiş

            smtp.EnableSsl = true; //Bu değer şirket mailileri için false olarak işaretlenmesi gerekebiliyor.
            smtp.Send(msg);

            return true;
        }
    }
}
