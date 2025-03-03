using MailKit.Net.Smtp;
using MimeKit;
using Serilog;

namespace Prod.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
}

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        using var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("cobro", "no-reply@isntrui.ru"));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message
        };

        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.selcloud.ru", 1127, true);
                await client.AuthenticateAsync("3221", "1y6SAoKlw9m0aBcjJY");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception e)
        {
            Log.Error("мобильщики идиоты, кинули фигня почту : " + e);
        }
    }
}
