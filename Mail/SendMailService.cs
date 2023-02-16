using System;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ElectronicShop.Mail;

public class MailSettings
{
    public string Mail { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
}
//Dịch vụ gửi mail
public class SendMailService : IEmailSender
{
    private readonly MailSettings _mailSettings;
    private readonly ILogger<SendMailService> _logger;
    // mailSetting được Inject qua dịch vụ hệ thống
    // Có inject Logger để xuất log
    public SendMailService(IOptions<MailSettings> mailSettings, ILogger<SendMailService> logger)
    {
        _mailSettings = mailSettings.Value;
        _logger = logger;
        _logger.LogInformation("Create SendMailService with : "+ mailSettings.Value.DisplayName+" + " + _mailSettings.Mail);
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
        message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = htmlMessage;
        message.Body = builder.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        try
        {
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(message);
        }
        catch (System.Exception ex)
        {
            // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
            System.IO.Directory.CreateDirectory("mailSave");
            var emailSaveFile = string.Format(@"mailSave/{0}.eml", Guid.NewGuid());
            await message.WriteToAsync(emailSaveFile);

            _logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailSaveFile);
            _logger.LogError(ex.Message);
        }
        smtp.Disconnect(true);
        _logger.LogInformation("send mail to: " + email);
    }
}