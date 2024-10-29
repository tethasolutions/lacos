using System.Net.Mail;
using System.Net;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public interface ISharedService
{
    Task SendMessage(string receiver, string CC, string subject, string body, Attachment attachments = null, bool isBodyHtml = true);
}

public class SharedService : ISharedService
{

    public SharedService()
    {
    }

    public async Task SendMessage(string receiver, string CC, string subject, string body, Attachment attachment = null, bool isBodyHtml = true)
    {
        SmtpClient SmtpClient = new SmtpClient("smtp.send" + "grid.net", 587)
        {
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("apikey", "S" + "G." + "C1sV_IiIQ_CZCDaF_4Bp3w" + ".8dJAZV" + "7Dj-DqYM-rPPeWIzz2eDkdmgGmtNIc9mQkmhE"),
            EnableSsl = false
        };

        string sender = "report@lacosgroup.it";

        var message = new MailMessage(sender, receiver, subject, body)
        {
            IsBodyHtml = isBodyHtml
        };

        message.CC.Add(new MailAddress(sender));

        if (CC != "")
        {
            message.CC.Add(new MailAddress(CC));
        }

        if (attachment != null)
        {
            message.Attachments.Add(attachment);
        }

        SmtpClient.Send(message);
    }
}